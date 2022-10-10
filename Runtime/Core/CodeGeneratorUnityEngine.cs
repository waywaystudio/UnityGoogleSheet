using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;
using Wayway.Engine.UnityGoogleSheet.Core.IO;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class CodeGeneratorUnityEngine : ICodeGenerator
    {
        public CodeGeneratorUnityEngine(SheetInfo info)
        {
            sheetInfo = info;
        }
        
        private SheetInfo sheetInfo;
        
        public string GenerateForm => defaultForm;
        
        public string Generate()
        {
            var @namespace = sheetInfo.sheetFileName;
            var className = sheetInfo.sheetName;
            
            TypeMap.Init();
            
            WriteCommonLoad();
            WriteLoadFunction();

            WriteAssembly(new [] {
                "System",
                "System.Collections.Generic",
                "System.Reflection",
                "UnityEngine",
                "Wayway.Engine.UnityGoogleSheet.Core",
                "Wayway.Engine.UnityGoogleSheet.Core.Attribute",
                "Wayway.Engine.UnityGoogleSheet.Core.Exception",
                "Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2"
            }, sheetInfo.sheetTypes, sheetInfo.isEnumChecks);
            
            WriteNamespace(@namespace);
            WriteSpreadSheetData(sheetInfo.spreadSheetID, sheetInfo.sheetID);
            WriteTypes(sheetInfo.sheetTypes, sheetInfo.sheetVariableNames, sheetInfo.isEnumChecks);
            WriteWriteFunction(className);
            WriteClassReplace(className);
            
            Debug.Log($"Generate <color=green><b>{className}.cs</b></color> Complete");
            
            return GenerateForm;
        }

        public string CommonLoad() => $@"

        public static (List<@class> list, Dictionary<@keyType, @class> map) CommonLoad(Dictionary<string, Dictionary<string, List<string>>> jsonObject, bool forceReload)
        {{
            var map = new Dictionary<@keyType, @class>();
            var list = new List<@class>();
            TypeMap.Init();
            var fields = typeof(@class).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<(string original, string propertyName, string type)> typeInfos = new (); 
            var rows = new List<List<string>>();
            var sheet = jsonObject[""@class""];

            foreach (var column in sheet.Keys)
            {{
                var split = column.Replace("" "", null).Split(':');
                var columnField = split[0];
                var columnType = split[1];

                typeInfos.Add((column, columnField, columnType));
                var typeValues = sheet[column];
                rows.Add(typeValues);
            }}

            // 실제 데이터 로드
            if (rows.Count != 0)
            {{
                var rowCount = rows[0].Count;
                for (var i = 0; i < rowCount; i++)
                {{
                    var instance = new @class();
                    for (var j = 0; j < typeInfos.Count; j++)
                    {{
                        try
                        {{
                            var typeInfo = TypeMap.StrMap[typeInfos[j].type];
                            //int, float, List<..> etc
                            var type = typeInfos[j].type;
                            if (type.StartsWith("" < "") && type.Substring(1, 4) == ""Enum"" && type.EndsWith("">""))
                            {{
                                 Debug.Log(""It's Enum"");
                            }}

                            var readValue = TypeMap.Map[typeInfo].Read(rows[j][i]);
                            fields[j].SetValue(instance, readValue);

                        }}
                        catch (Exception e)
                        {{
                            if (e is UGSValueParseException)
                            {{
                                Debug.LogError(""<color=red> UGS Value Parse Failed! </color>"");
                                Debug.LogError(e);
                                return (null, null);
                            }}

                            //enum parse
                            var type = typeInfos[j].type;
                            type = type.Replace(""Enum<"", null);
                            type = type.Replace("">"", null);

                            var readValue = TypeMap.EnumMap[type].Read(rows[j][i]);
                            fields[j].SetValue(instance, readValue); 
                        }}
                      
                    }}
                    list.Add(instance); 
                    map.Add(instance.{ToPascalCasing(sheetInfo.sheetVariableNames[0])}, instance);
                }}

                if(isLoaded == false || forceReload)
                {{ 
                    @classList = list;
                    @classMap = map;
                    isLoaded = true;
                }}
            }}
 
            return (list, map); 
        }}
";
        
        private string defaultForm = @"
/*     ===== Do not touch this. Auto Generated Code. =====    */
/*     If you want custom code generation modify this => 'CodeGeneratorUnityEngine.cs'  */
@assemblies
namespace @namespace
{
    [TableStruct]
    public partial class @class : ITable
    {
        public delegate void OnLoadedFromGoogleSheets(List<@class> loadedList, Dictionary<@keyType, @class> loadedDictionary);

        private static bool isLoaded;
        private static string spreadSheetID = ""@spreadSheetID""; // it is file id
        private static string sheetID = ""@sheetID""; // it is sheet id
        private static UnityFileReader reader = new ();

/* Your Loaded Data Storage. */
    
        public static Dictionary<@keyType, @class> @classMap = new ();  
        public static List<@class> @classList = new ();   

        /// <summary>
        /// Get @class List 
        /// Auto Load
        /// </summary>
        public static List<@class> GetList()
        {
           if (isLoaded == false) Load();
           return @classList;
        }

        /// <summary>
        /// Get @class Dictionary, keyType is your sheet A1 field type.
        /// - Auto Load
        /// </summary>
        public static Dictionary<@keyType, @class>  GetDictionary()
        {
           if (isLoaded == false) Load();
           return @classMap;
        }

    

/* Fields. */
@types  

#region functions

@loadFunction
@CommonLoad 
@writeFunction  

#endregion

#region OdinInspectorExtensions
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button(""UploadToSheet"")]
        public void Upload()
        {
            Write(this);
        }    
#endif 
#endregion
    }
}
        ";

        private void WriteTypes(string[] types, string[] fieldNames, IReadOnlyList<bool> isEnum)
        {
            if (types != null)
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                
                for (var i = 0; i < types.Count(); i++)
                {
                    if (isEnum[i] == false)
                    {
                        var targetType = types[i];
                        var targetField = fieldNames[i];
                        TypeMap.StrMap.TryGetValue(targetType, out var outType);
                        
                        if (outType == null)
                        {
                            Debug.Log("<color=#00ff00><b>-------UGS IMPORTANT ERROR DEBUG---------</b></color>");
                            var debugTypes = string.Join("  ", sheetInfo.sheetTypes);
                            Debug.LogError($"<color=white><b>Error Sheet Name => </b></color>{sheetInfo.sheetFileName}.{sheetInfo.sheetName}");
                            Debug.LogError($"<color=white><b>Your type list => </b></color> => {debugTypes}");
                            Debug.LogError($"<color=#00ff00><b>error field =>:</b></color> {targetField} : {sheetInfo.sheetTypes[i]}");
                            
                            throw new TypeParserNotFoundException("Type Parser Not Found, You made your own type parser? check custom type document on gitbook document.");
                        }
                        
                        builder.AppendLine($"\t\tpublic {outType.Namespace}.{GetCSharpRepresentation(TypeMap.StrMap[types[i]], true)} {ToPascalCasing(fieldNames[i])};");
                    }
                    else
                    {
                        var str = types[i];
                        
                        str = str.Replace("<", null);
                        str = str.Replace(">", null);
                        str = str.Replace(" ", null);
                        str = str.Remove(0, 4);
                        
                        builder.AppendLine($"\t\tpublic {GetCSharpRepresentation(TypeMap.EnumMap[str].Type, true)} {ToPascalCasing(fieldNames[i])};");
                    }
                }
                
                defaultForm = defaultForm.Replace("@types", builder.ToString());
                defaultForm = defaultForm.Replace("@keyType", types[0]);
            }
        }

        private static string GetCSharpRepresentation(Type t, bool trimArgCount)
        {
            if (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments().ToList();

                return GetCSharpRepresentation(t, trimArgCount, genericArgs);
            }

            return t.Name;
        }

        private static string GetCSharpRepresentation(Type t, bool trimArgCount, IList<Type> availableArguments)
        {
            if (t.IsGenericType)
            {
                var value = t.Name;
                if (trimArgCount && value.IndexOf("`", StringComparison.Ordinal) > -1)
                {
                    value = value[..value.IndexOf("`", StringComparison.Ordinal)];
                }

                if (t.DeclaringType != null)
                    value = GetCSharpRepresentation(t.DeclaringType, trimArgCount, availableArguments) + "+" + value;
                var argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                
                for (var i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ", ";
                    argString += GetCSharpRepresentation(availableArguments[0], trimArgCount);
                    availableArguments.RemoveAt(0);
                }
                
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }

                return value;
            }

            return t.Name;
        }

        private void WriteWriteFunction(string @class)
        {
            var writeFunction = $@"
        public static void Write(@class data, Action<WriteObjectResult> onWriteCallback = null)
        {{ 
            TypeMap.Init();
            var fields = typeof({@class}).GetFields(BindingFlags.Public | BindingFlags.Instance);
            var dataList = new string[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {{
                var type = fields[i].FieldType;                
                var writeRule = type.IsEnum ? TypeMap.EnumMap[type.Name].Write(fields[i].GetValue(data)) 
                                            : TypeMap.Map[type].Write(fields[i].GetValue(data)); 

                dataList[i] = writeRule; 
            }}             
#if UNITY_EDITOR
            UnityPlayerWebRequest.Instance.WriteObject(new WriteObjectReqModel(spreadSheetID, sheetID, dataList[0], dataList), null, onWriteCallback);
#endif
        }} 
        ";

            defaultForm = defaultForm.Replace("@writeFunction", writeFunction);
        }
        
        private void WriteNamespace(string @namespace)
        {
            defaultForm = defaultForm.Replace("@namespace", @namespace);
        }
        
        private void WriteClassReplace(string @class)
        {
            defaultForm = defaultForm.Replace("@class", @class);
        }

        private void WriteSpreadSheetData(string spreadID, string sheetID)
        {
            defaultForm = defaultForm.Replace("@spreadSheetID", spreadID);
            defaultForm = defaultForm.Replace("@sheetID", sheetID);
        }

        private void WriteAssembly(string[] assemblies, string[] types = null, bool[] isEnums = null)
        {
            if (assemblies != null)
            {
                var builder = new StringBuilder();
                foreach (var assembly in assemblies)
                    builder.AppendLine($"using {assembly};");
                builder.AppendLine("@assemblies");
                defaultForm = defaultForm.Replace("@assemblies", builder.ToString());
            }

            /* Enum */
            if (types != null && isEnums != null)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    var isEnum = isEnums[i];
                    type = type.Replace(" ", null);
                    type = type.Replace("Enum<", null);
                    type = type.Replace(">", null);
                    
                    if (isEnum && !string.IsNullOrEmpty(TypeMap.EnumMap[type].NameSpace))
                    {
                        builder.AppendLine($"using {TypeMap.EnumMap[type].NameSpace};");
                    }
                }
                
                defaultForm = defaultForm.Replace("@assemblies", builder.ToString());
            }
            else
            {
                defaultForm = defaultForm.Replace("@assemblies", null);
            }
        }

        private void WriteCommonLoad()
        {
            var builder = new StringBuilder();
            builder.Append(CommonLoad());
            defaultForm = defaultForm.Replace("@CommonLoad", builder.ToString());
        }

        private void WriteLoadFunction()
        {
            var builder = new StringBuilder();
            builder.Append($@"
        public static void Load(bool forceReload = false)
        {{
            if(isLoaded && forceReload == false)
            {{
#if UGS_DEBUG
                 Debug.Log(""@class is already loaded! if you want reload then, forceReload parameter set true"");
#endif
                 return;
            }}

            var text = reader.ReadData(""@namespace""); 
            if (text != null)
            {{
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReadSpreadSheetResult>(text);
                if (result != null) CommonLoad(result.jsonObject, forceReload);

                if(!isLoaded)
                    isLoaded = true;
            }}
      
        }}
");
            defaultForm = defaultForm.Replace("@loadFunction", builder.ToString());
        }

        private string ToCamelCasing(string target)
        {
            if(!string.IsNullOrEmpty(target) && target.Length > 1)
            {
                return char.ToLowerInvariant(target[0]) + target[1..];
            }
            
            return string.IsNullOrEmpty(target) ? string.Empty  
                                                : target.ToLowerInvariant();
        }

        private string ToPascalCasing(string target)
        {
            var replace = target.ToLower().Replace("_", " ");
            var info = CultureInfo.CurrentCulture.TextInfo;
            replace = info.ToTitleCase(replace).Replace(" ", string.Empty);

            return replace;
        }
    }
}