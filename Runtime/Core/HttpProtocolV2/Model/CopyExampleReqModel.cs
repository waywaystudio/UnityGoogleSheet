// ReSharper disable ClassNeverInstantiated.Global

namespace Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2
{
    public class CopyExampleReqModel : Model
    {
        public CopyExampleReqModel()
        {
            instruction = (int)EInstruction.COPY_EXAMPLE;
        }
    }
}
