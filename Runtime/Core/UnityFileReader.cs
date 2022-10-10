using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class UnityFileReader
    {
        public string ReadData(string fileName)
        {
            return Application.isPlaying == false ? EditorAssetLoad(fileName) 
                                                  : RuntimeAssetLoad(fileName);
        }

        private string ToUnityResourcePath(string path)
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
        
        public string EditorAssetLoad(string fileName)
        {
#if UNITY_EDITOR
            var combine = Path.Combine(UgsConfig.Instance.JsonDataPath, fileName);
            combine = combine.Replace("\\", "/");
            var filePath = ToUnityResourcePath(combine);

            var textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset != null)
            {
                return textAsset.text;
            }
#endif
            throw new System.Exception($"UGS File Read Failed (path = {"UGS.Data/" + fileName})");
        }

        public string RuntimeAssetLoad(string fileName)
        {
            var combine = Path.Combine(UgsConfig.Instance.JsonDataPath, fileName);
            combine = combine.Replace("\\", "/");
            var filePath = ToUnityResourcePath(combine);

            filePath = filePath.Replace("Resources/", null);

            var textAsset = Resources.Load<TextAsset>(filePath);
            return textAsset == null ? null : textAsset.text;
        }
    }
}