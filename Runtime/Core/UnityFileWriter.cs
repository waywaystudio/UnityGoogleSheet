#if UNITY_EDITOR

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class UnityFileWriter
    {
        public static void WriteCSharpScript(string @namespace, string className, string content)
        {
            var directoryPath = $"{UgsConfig.Instance.CSharpScriptPath}/{@namespace}";
            var targetPath = $"{directoryPath}/{className}.cs";

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            System.IO.File.WriteAllText(targetPath, content);
        }

        public static void WriteJsonData(string dataName, string content)
        {
            var targetPath = $"{UgsConfig.Instance.JsonDataPath}/{dataName}.json";
            
            if (!System.IO.Directory.Exists(UgsConfig.Instance.JsonDataPath))
            {
                System.IO.Directory.CreateDirectory(UgsConfig.Instance.JsonDataPath);
            }
            
            UnityEngine.Debug.Log($"<color=green>{dataName}.json</color> Created");
            System.IO.File.WriteAllText(targetPath, content);
        }

        public static void WriteScriptableObjectScript(string @namespace, string className, string content)
        {
            var directoryPath = $"{UgsConfig.Instance.ScriptableObjectScriptPath}/{@namespace}";
            var targetPath = $"{directoryPath}/{className}.cs";

            if (!System.IO.Directory.Exists(UgsConfig.Instance.ScriptableObjectScriptPath))
            {
                System.IO.Directory.CreateDirectory(UgsConfig.Instance.ScriptableObjectScriptPath);
            }

            System.IO.File.WriteAllText(targetPath, content);

            UgsUtility.UpdateTableObject(className, directoryPath);
        }
    }
}
#endif