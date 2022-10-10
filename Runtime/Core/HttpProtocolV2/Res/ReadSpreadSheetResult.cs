// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global

using System.Collections.Generic;

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class ReadSpreadSheetResult : Response 
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public Dictionary<string, Dictionary<string, List<string>>> jsonObject;
        public string spreadSheetName;
        public string spreadSheetID;
        public List<string> sheetIDList;
        public List<EFileType> tableTypes;  
    }
}
