// ReSharper disable InconsistentNaming

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class WriteObjectReqModel : Model
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public string fileID;
        public string sheetID;
        public string key;
        [Newtonsoft.Json.JsonProperty("value")]
        public string[] values; 
        public WriteObjectReqModel(string fileID, string sheetID, string key, string[] values)
        {
            this.fileID = fileID;
            this.sheetID = sheetID;
            this.key = key;
            this.values = values;
            
            instruction = (int)EInstruction.WRITE_OBJECT;
        }
    }
}
