// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public interface IHttpProtocol
    {
        void GetDriveDirectory(GetDriveDirectoryReqModel mdl, System.Action<System.Exception> errResponse, System.Action<GetDriveFolderResult> callback);
        void ReadSpreadSheet(ReadSpreadSheetReqModel mdl, System.Action<System.Exception> errResponse, System.Action<ReadSpreadSheetResult> callback);
        void WriteObject(WriteObjectReqModel mdl, System.Action<System.Exception> errResponse, System.Action<WriteObjectResult> callback);
        void CreateDefaultSheet(CreateDefaultReqModel mdl, System.Action<System.Exception> errResponse, System.Action<CreateDefaultSheetResult> callback);
        void CopyExample(CopyExampleReqModel mdl, System.Action<System.Exception> errResponse, System.Action<CreateExampleResult> callback);
        void CopyFolder(CopyFolderReqModel mdl, System.Action<System.Exception> errResponse, System.Action<CreateExampleResult> callback);
    }
}