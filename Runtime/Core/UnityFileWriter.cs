#if UNITY_EDITOR

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class UnityFileWriter
    {
        public static void WriteCSharpScript(string @namespace, string className, string content)
        {
            var directoryPath = $"{UgsConfig.Instance.CSharpScriptPath}/{@namespace}";

            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);

            System.IO.File.WriteAllText($"{directoryPath}/{className}.cs", content);
        }

        public static void WriteJsonData(string dataName, string content)
        {
            var directoryPath = UgsConfig.Instance.JsonDataPath;
            
            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);
            
            UnityEngine.Debug.Log($"<color=green>{dataName}.json</color> Created");
            System.IO.File.WriteAllText($"{directoryPath}/{dataName}.json", content);
        }

        public static void WriteScriptableObjectScript(string @namespace, string className, string content)
        {
            var directoryPath = $"{UgsConfig.Instance.ScriptableObjectScriptPath}/{@namespace}";
            
            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);

            System.IO.File.WriteAllText($"{directoryPath}/{className}.cs", content);
            UgsUtility.UpdateTableObject(className, directoryPath);
        }
    }
}
#endif