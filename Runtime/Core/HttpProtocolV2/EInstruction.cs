// ReSharper disable InconsistentNaming

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{ 
    public enum EInstruction
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        SEARCH_GOOGLE_DRIVE = 0,
        READ_SPREADSHEET = 1,
        WRITE_OBJECT = 2,
        CREATE_DEFAULT_TABLE = 101,
        COPY_EXAMPLE = 102,
        COPY_FOLDER = 103
    }
}
