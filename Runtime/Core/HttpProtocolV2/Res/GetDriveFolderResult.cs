// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Collections.Generic;

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public partial class GetDriveFolderResult : Response
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public List<string> fileId;
        public List<string> fileName;
        public List<int> fileType;
        public List<string> url; 
    }
}
