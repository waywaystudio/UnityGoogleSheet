using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

using JsonResult = System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>;

#if UNITY_EDITOR
namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public static class UgsEditorUtility
    {
        public static List<T> LoadFromJson<T>(string jsonFileName) where T : class, new() 
        {
            var text = ReadData(jsonFileName);
            if (text is not null)
            {
                var output = Newtonsoft.Json.JsonConvert.DeserializeObject<ReadSpreadSheetResult>(text);
                if (output != null)
                {
                    Debug.Log($"Load {jsonFileName}.json Success! From <color=green><b>{typeof(T).Name}</b></color>");
                    
                    return CommonLoad<T>(output.jsonObject);
                }
            }

            return null;
        }

        private static string ReadData(string fileName)
        {
            var combine = Path.Combine(UgsConfig.Instance.JsonDataPath, fileName);
            combine = combine.Replace("\\", "/");
            var filePath = ToUnityResourcePath(combine);

            var textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset != null)
            {
                return textAsset.text;
            }
            
            throw new Exception($"UGS File Read Failed (path = {"UGS.Data/" + fileName})");
        }
        
        private static string ToUnityResourcePath(string path)
        {
            var paths = path.Split('/');
            var link = false;
            var newPath = new List<string>();
            foreach(var value in paths)
            {
                if (value == "Resources")
                {
                    link = true;
                    continue;
                }

                if (link)
                {
                    newPath.Add(value);
                }
            }
             
            return string.Join("/", newPath);
        }

        private static List<T> CommonLoad<T>(JsonResult jsonObject) where T : class, new()
        {
            TypeMap.Init();
            
            var list = new List<T>();
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            var typeInfos = new List<(string original, string propertyName, string type)>(); 
            var rows = new List<List<string>>();
            var sheet = jsonObject[typeof(T).Name];

            foreach (var column in sheet.Keys)
            {
                var split = column.Replace(" ", null).Split(':');
                var columnField = split[0];
                var columnType = split[1];

                typeInfos.Add((column, columnField, columnType));
                var typeValues = sheet[column];
                rows.Add(typeValues);
            }

            if (rows.Count != 0)
            {
                var rowCount = rows[0].Count;
                for (var i = 0; i < rowCount; i++)
                {
                    var instance = new T();
                    for (var j = 0; j < typeInfos.Count; j++)
                    {
                        try
                        {
                            var typeInfo = TypeMap.StrMap[typeInfos[j].type];
                            var type = typeInfos[j].type;
                            
                            if (type.StartsWith(" < ") && type.Substring(1, 4) == "Enum" && type.EndsWith(">"))
                            {
                                 Debug.Log("It's Enum");
                            }

                            var readValue = rows[j][i] is "" ? GetDefault(typeInfo)
                                                             : TypeMap.Map[typeInfo].Read(rows[j][i]);
                            
                            fields[j].SetValue(instance, readValue);
                        }
                        catch (Exception e)
                        {
                            if (e is UGSValueParseException)
                            {
                                Debug.LogError("<color=red> UGS Value Parse Failed! </color>");
                                Debug.LogError(e);
                                
                                return null;
                            }

                            var type = typeInfos[j].type;
                            type = type.Replace("Enum<", null);
                            type = type.Replace(">", null);

                            var readValue = TypeMap.EnumMap[type].Read(rows[j][i]);
                            fields[j].SetValue(instance, readValue); 
                        }
                    }
                    
                    list.Add(instance);
                }
            }
 
            return list;
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
#endif