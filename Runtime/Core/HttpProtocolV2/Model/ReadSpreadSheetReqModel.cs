// ReSharper disable InconsistentNaming

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class GetDriveDirectoryReqModel : Model
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public string folderId;

        public GetDriveDirectoryReqModel(string folderId)
        {
            this.folderId = folderId;
            
            instruction = (int)EInstruction.SEARCH_GOOGLE_DRIVE;
        }
    }
}
