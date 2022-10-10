// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global

using Newtonsoft.Json.Linq;

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class Response
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public bool hasError() { return error != null; }
        public EInstruction instruction;
        public ErrorResponse error;
    }

    public class ErrorResponse
    {
        /*
         * DO NOT change enum name and style
         * Sync with AppScript
         */
        public string message;
        public JObject eReq;
        public string eType; 
        public string eStackTrace;
    }
}
