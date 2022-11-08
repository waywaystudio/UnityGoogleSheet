using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class UgsUtility
    {
        /// <summary>
        /// Get all Subclass Of
        /// </summary>
        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            var type = parent;
            var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
            return types;
        }
        
        public static string ToQueryString(object data, string password = null)
        {
            var fields = (from p in data.GetType().GetFields()
                where p.GetValue(data) != null
                select p).ToList();

            var properties = from p in data.GetType().GetFields()
                where p.GetValue(data) != null
                select p.Name + "=" + Uri.EscapeUriString(p.GetValue(data).ToString());

            return "?" + string.Join("&", properties.ToArray()) + $"&password={Uri.EscapeUriString(password)}";
        }
        
        public static bool FindScriptableObject(string className, string folderPath, out ScriptableObject result)
        {
            result = GetScriptableObject(className, folderPath);

            return result;
        }

        public static string GetScriptableObjectFilePath(string className, string folderPath)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(folderPath)) folderPath = "Assets";
            
            var gUIDs = UnityEditor.AssetDatabase.FindAssets($"t:{className}", new [] { folderPath });

            if (gUIDs == null || gUIDs.Length == 0)
            {
                return null;
            }
            
            var targetIndex = 0;
            if (gUIDs.Length > 1)
            {
                for (var i = 0; i < gUIDs.Length; ++i)
                {
                    var assetName = UnityEditor.AssetDatabase.GUIDToAssetPath(gUIDs[i]).Replace(folderPath, "")
                        .Replace(".asset", "");
                    
                    targetIndex = i;
                        
                    Debug.LogWarning($"Multiple Founded :: count : {gUIDs.Length} " +
                                     $"Select Asset by Exactly SameName :: {assetName}");
                }
            }
            
            return UnityEditor.AssetDatabase.GUIDToAssetPath(gUIDs[targetIndex]);
#elif !UNITY_EDITOR
            return string.Empty;
#endif
        }
        
        public static ScriptableObject GetScriptableObject(string className, string folderPath)
        {
#if UNITY_EDITOR
            var assetPath = GetScriptableObjectFilePath(className, folderPath);
            return UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject)) as ScriptableObject;
#elif !UNITY_EDITOR
            return null;
#endif
        }
        
        public static List<Object> GetObjectList(string className, string folderPath, string filter)
        {
            var result = new List<Object>();
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(folderPath)) folderPath = "Assets";
            if (string.IsNullOrEmpty(filter)) filter = $"t:{className}";

            var gUIDs = UnityEditor.AssetDatabase.FindAssets(filter, new [] { folderPath });

            foreach (var x in gUIDs)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(x);
                var data = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

                if (!result.Contains(data))
                    result.Add(data);
            }
#endif
            return result;
        }

#if UNITY_EDITOR
        public static List<UnityEditor.MonoScript> GetMonoScriptList(string folderPath, string filter)
        {
            var result = new List<UnityEditor.MonoScript>();

            if (string.IsNullOrEmpty(folderPath)) folderPath = "Assets";
            if (string.IsNullOrEmpty(filter)) filter = $"t:MonoScript";

            var gUIDs = UnityEditor.AssetDatabase.FindAssets(filter, new [] { folderPath });

            foreach (var x in gUIDs)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(x);
                var data = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEditor.MonoScript)) as UnityEditor.MonoScript;

                if (!result.Contains(data) && data is not null)
                    result.Add(data);
            }

            return result;
        }
#endif
        
        public static ScriptableObject CreateScriptableObject(string className, string folderPath)
        {
#if UNITY_EDITOR
            if (folderPath == null)
            {
                Debug.LogError("at least one of a Parameter value is <b><color=red>NULL!</color></b>");
                return null;
            }
            
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            if (FindScriptableObject(className, folderPath, out var result))
            {
                Debug.Log($"{className} ScriptableObject is already Exist. Skip Creation");
                return null;
            }

            var data = ScriptableObject.CreateInstance(className);
            var uniqueName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{className}.asset");

            UnityEditor.AssetDatabase.CreateAsset(data, uniqueName);

            Debug.Log($"Create <b><color=green>{uniqueName}</color></b>");

            return data;
#elif !UNITY_EDITOR
            return null;
#endif
        }
        
        public static void InvokeFunction(Object targetObject, string functionName)
        {
            var scriptableObjectType = targetObject.GetType();
            var info = scriptableObjectType.GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
            
            Debug.Log($"Type : {scriptableObjectType}, is Info ? : {info != null}");
            
            if (info != null)
            {
                info.Invoke(targetObject, null);
            }
        }
       
        public static void UpdateTableObject(string className, string directoryPath)
        {
#if UNITY_EDITOR
            if (FindScriptableObject(className, directoryPath, out var result))
            {
                InvokeFunction(result, "LoadFromJson");
                
                UnityEditor.EditorUtility.SetDirty(result);
                UnityEditor.AssetDatabase.Refresh();
            }
#endif
        }
    }
}
