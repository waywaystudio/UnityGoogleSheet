#if UNITY_EDITOR
// ReSharper disable InconsistentNaming

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;
using Wayway.Engine.UnityGoogleSheet.Core.IO;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class UnityGSParser
    {
        public static void ParseJsonData(ReadSpreadSheetResult sheetJsonData)
        {
            var jsonData = GenerateData(sheetJsonData);
            var existJsonData = GetExistJsonFileText(sheetJsonData.spreadSheetName);

            if (IsSameHashCode(jsonData, existJsonData))
            {
                Debug.Log($"<color=green>{sheetJsonData.spreadSheetName}</color> is nothing changed. Writing Process Skipped");
                return;
            }

            UnityFileWriter.WriteJsonData(sheetJsonData.spreadSheetName, jsonData);
        }

        public static void ParseSheet(ReadSpreadSheetResult sheetJsonData) => ParseSheet(sheetJsonData, null);
        public static void ParseSheet(ReadSpreadSheetResult sheetJsonData, string targetWorkSheetName)
        {
            if (!UgsConfig.Instance.doGenerateCSharpScript && 
                !UgsConfig.Instance.doGenerateScriptableObject) 
                return;
            
            var workSheet = targetWorkSheetName is null ? null 
                                                        : sheetJsonData.jsonObject[targetWorkSheetName];

            if (targetWorkSheetName != null && !sheetJsonData.jsonObject.ContainsKey(targetWorkSheetName))
            {
                Debug.LogError($"There is no Worksheet name of <color=red><b>{targetWorkSheetName}</b></color> in SpreadSheet");
                return;
            }
            
            var count = 0;
            
            foreach (var sheet in sheetJsonData.jsonObject)
            {
                var jsonObject =
                    JObject.Parse(GetExistJsonFileText(sheetJsonData.spreadSheetName));

                Debug.Log(JsonConvert.SerializeObject(sheet, Formatting.Indented));
                Debug.Log(jsonObject.GetValue("jsonObject"));
                
                // Debug.Log(GetExistJsonFileText(sheetJsonData.spreadSheetName).GetHashCode());
                
                // load spreadSheet json file.
                // load json file.worksheet
                // compare worksheet.hashcode with sheetJsonData.jsonObject.hashcode
                // if (sheet.GetHashCode())
                
                if (workSheet != null && sheet.Key != targetWorkSheetName)
                {
                    count++;
                    continue;
                }

                var sheetInfoTypes = new string[sheet.Value.Count];
                var sheetInfoNames = new string[sheet.Value.Count];
                var isEnum = new bool[sheet.Value.Count];
                var i = 0;
                
                foreach (var split in sheet.Value.Select(type => type.Key)
                                                 .Select(id => id.Replace(" ", null)
                                                 .Split(':')))
                {
                    sheetInfoTypes[i] = split[1];
                    sheetInfoNames[i] = split[0];
                    isEnum[i] = split[1].Contains("Enum<");

                    i++;
                }
                
                var info = new SheetInfo
                {
                    spreadSheetID = sheetJsonData.spreadSheetID,
                    sheetID = sheetJsonData.sheetIDList[count],
                    sheetFileName = sheetJsonData.spreadSheetName,
                    sheetName = sheet.Key,
                    sheetTypes = sheetInfoTypes,
                    sheetVariableNames = sheetInfoNames,
                    isEnumChecks = isEnum
                };
            
                if (UgsConfig.Instance.doGenerateCSharpScript)
                {
                    var result = GenerateCSharpCode(info);
                    UnityFileWriter.WriteCSharpScript(info.sheetFileName, $"{info.sheetName}", result);
                }

                if (UgsConfig.Instance.doGenerateScriptableObject)
                {
                    var result = GenerateScriptableObjectCode(info);
                    UnityFileWriter.WriteScriptableObjectScript(info.sheetFileName,$"{info.sheetName}{UgsConfig.Instance.Suffix}", result);
                }

                count++;
            }
            
            UnityEditor.AssetDatabase.Refresh();
        }
        
        private static string GenerateData(ReadSpreadSheetResult tableResult)
        {
            var dataGen = new DataGenerator(tableResult);
            var result = dataGen.Generate();
            
            return result;
        }

        private static string GenerateCSharpCode(SheetInfo info)
        {
            var sheetGenerator = new CodeGeneratorUnityEngine(info);
            var result = sheetGenerator.Generate();
            
            return result;
        }

        private static string GenerateScriptableObjectCode(SheetInfo info)
        {
            var sheetGenerator = new CodeGeneratorScriptableObject(info);
            var result = sheetGenerator.Generate();
            
            return result;
        }

        private static string GetExistJsonFileText(string spreadSheetName)
        {
            var jsonFilePath = $"{UgsConfig.Instance.JsonDataPath}/{spreadSheetName}.json";
            var jsonFileText = File.ReadAllText(jsonFilePath);

            return jsonFileText;
        }

        private static bool IsSameHashCode(string one, string another)
        {
            return one.GetHashCode() == another.GetHashCode();
        }
    }
}
#endif