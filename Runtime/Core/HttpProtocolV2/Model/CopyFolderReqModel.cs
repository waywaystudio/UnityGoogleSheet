// ReSharper disable InconsistentNaming

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class CopyFolderReqModel : Model
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        
        public string folderId;

        public CopyFolderReqModel(string folderId)
        {
            this.folderId = folderId;
            
            instruction = (int)EInstruction.COPY_FOLDER;
        }
    }
}
