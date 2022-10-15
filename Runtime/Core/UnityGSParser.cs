#if UNITY_EDITOR
// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;
using Wayway.Engine.UnityGoogleSheet.Core.IO;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class UnityGSParser
    {
        private static bool compareHash => UgsConfig.Instance.CompareHashCode;
        private static bool isSkipGenerateScript => !UgsConfig.Instance.DoGenerateCSharpScript &&
                                                    !UgsConfig.Instance.DoGenerateScriptableObject;
        
        public static void ParseJsonData(ReadSpreadSheetResult sheetJsonData)
        {
            var importJsonData = GenerateData(sheetJsonData);

            if (compareHash)
            {
                var existJsonData = GetExistJsonFileText(sheetJsonData.spreadSheetName);

                if (IsSameHashCode(importJsonData, existJsonData))
                {
                    Debug.Log($"{sheetJsonData.spreadSheetName} is nothing changed. Writing Process Skipped");
                    return;
                }
            }

            UnityFileWriter.WriteJsonData(sheetJsonData.spreadSheetName, importJsonData);
        }

        public static void ParseSheet(ReadSpreadSheetResult sheetJsonData, string targetWorkSheetName = null)
        {
            if (isSkipGenerateScript) 
                return;

            var sheetInfoList = GetSheetInformationList(sheetJsonData, targetWorkSheetName);

            sheetInfoList.ForEach(x =>
            {
                if (UgsConfig.Instance.DoGenerateCSharpScript)
                {
                    var result = GenerateCSharpCode(x);
                    UnityFileWriter.WriteCSharpScript(x.sheetFileName, $"{x.sheetName}", result);
                }
                
                if (UgsConfig.Instance.DoGenerateScriptableObject)
                {
                    var result = GenerateScriptableObjectCode(x);
                    UnityFileWriter.WriteScriptableObjectScript(x.sheetFileName,$"{x.sheetName}{UgsConfig.Instance.Suffix}", result);
                }
            });

            UnityEditor.AssetDatabase.Refresh();
        }
        
        private static IEnumerable<SheetInfo> GetSheetInformationList(ReadSpreadSheetResult sheetJsonData,
            string targetWorkSheetName = null)
        {
            var result = new List<SheetInfo>();
            var workSheet = targetWorkSheetName is null ? null 
                                                        : sheetJsonData.jsonObject[targetWorkSheetName];

            if (targetWorkSheetName != null && !sheetJsonData.jsonObject.ContainsKey(targetWorkSheetName))
            {
                Debug.LogError($"There is no Worksheet name of <color=red><b>{targetWorkSheetName}</b></color> in SpreadSheet");
                return null;
            }
            
            var count = 0;
            
            foreach (var sheet in sheetJsonData.jsonObject)
            {
                if (workSheet != null && sheet.Key != targetWorkSheetName)
                {
                    count++;
                    continue;
                }

                if (compareHash)
                {
                    var existedWorkSheetText = GetExistJsonWorkSheetText(sheetJsonData.spreadSheetName, sheet.Key);
                    var importWorkSheetText = JsonConvert.SerializeObject(sheet.Value, Formatting.Indented);
                    
                    if (IsSameHashCode(existedWorkSheetText, importWorkSheetText))
                    {
                        Debug.Log($"{sheet.Key} is nothing changed. Writing Process Skipped");
                    
                        count++;
                        continue;
                    }
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
                
                result.Add(info);
                count++;
            }

            return result;
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

        private static string GetExistJsonWorkSheetText(string spreadSheetName, string workSheetName)
        {
            var jsonSheetObject = (JObject)JObject.Parse(GetExistJsonFileText(spreadSheetName))
                                                  .GetValue("jsonObject");
            var jsonWorkSheet = jsonSheetObject?.GetValue(workSheetName);

            return jsonWorkSheet != null ? jsonWorkSheet.ToString() 
                                         : string.Empty;
        }

        private static bool IsSameHashCode(string one, string another)
        {
            return one.GetHashCode() == another.GetHashCode();
        }
    }
}
#endif