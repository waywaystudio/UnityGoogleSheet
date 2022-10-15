using Newtonsoft.Json;
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
       
        public string Generate() => JsonConvert.SerializeObject(info);
    }
}
