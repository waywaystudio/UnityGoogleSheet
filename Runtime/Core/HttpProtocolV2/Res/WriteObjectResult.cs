namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public partial class WriteObjectResult : Response
    {
        /// <summary>
        /// true = updated
        /// false = created new
        /// </summary>
        public bool isUpdate; 
    }
}
