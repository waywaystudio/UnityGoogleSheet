using Newtonsoft.Json;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

namespace Wayway.Engine.UnityGoogleSheet.Core.IO
{
    public class DataGenerator : ICodeGenerator
    {
        private ReadSpreadSheetResult info;
        public DataGenerator(ReadSpreadSheetResult info)
        {
             this.info = info;
        }
       
        public string Generate()
        {
            Debug.Log($"Generate {info.spreadSheetName}.Json Data Complete");
            
            return JsonConvert.SerializeObject(info, Formatting.Indented); 
        }
    }
}
