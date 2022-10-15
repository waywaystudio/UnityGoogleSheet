// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

using System;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public interface IHttpProtocol
    {
        void GetDriveDirectory(GetDriveDirectoryReqModel mdl, Action<System.Exception> errResponse, Action<GetDriveFolderResult> callback);
        void ReadSpreadSheet(ReadSpreadSheetReqModel mdl, Action<System.Exception> errResponse, Action<ReadSpreadSheetResult> callback);
        void WriteObject(WriteObjectReqModel mdl, Action<System.Exception> errResponse, Action<WriteObjectResult> callback);
        void CreateDefaultSheet(CreateDefaultReqModel mdl, Action<System.Exception> errResponse, Action<CreateDefaultSheetResult> callback);
        void CopyExample(CopyExampleReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback);
        void CopyFolder(CopyFolderReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback);
    }
}