#if UNITY_EDITOR || UNITY_BUILD
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEditor;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class UnityEditorWebRequest : IHttpProtocol
    {
        public static UnityEditorWebRequest Instance => instance ??= new UnityEditorWebRequest();
        private static UnityEditorWebRequest instance;
        public static string BaseUrl => UgsConfig.Instance.GoogleScriptUrl;

        private void Get<T>(string url, Action<System.Exception> errCallback, Action<T> callback) where T : Response
        { 
            try
            {
                EditorUtility.DisplayProgressBar("Request From Google Script..", "Please Wait a Second..", 1);
                
                var request = WebRequest.Create(url);
                request.Timeout = 300000;
                request.Credentials = CredentialCache.DefaultCredentials;
                var response = request.GetResponse();
                var statusCode = ((HttpWebResponse)response).StatusCode;
                
                if (statusCode == HttpStatusCode.RequestTimeout)
                {
                    EditorUtility.DisplayDialog("Timeout", "GoogleSheet Initialize Failed! Try Check Setting Window.", "ok");
                    errCallback?.Invoke(new System.Exception("TimeOut!"));
                }
                
                if (statusCode == HttpStatusCode.OK)
                {
                    using var dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        var reader = new StreamReader(dataStream);
                        var responseFromServer = reader.ReadToEnd();
                        var res = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseFromServer);
                        if (res != null && res.hasError())
                        {
                            throw new UGSWebError(res.error.message);
                        }

                        callback?.Invoke(res);
                    }
                }
                else 
                    errCallback?.Invoke(new System.Exception("Internal Error"));

                response.Close();
                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception e)
            {  
                EditorUtility.ClearProgressBar();
                switch (e)
                {
                    case WebException:
                    {
                        break;
                    }
                    case HttpRequestException:
                        EditorUtility.DisplayDialog("Http Request Failed",
                            "GoogleID Script Request Failed, Please Check Setting!", "OK");
                        break;
                    case UriFormatException:
                        EditorUtility.DisplayDialog("Invalid URI",
                            "Please Check Script URL Format in HamsterLib->UGS->Setting", "OK");
                        break;
                    case KeyNotFoundException:
                        EditorUtility.DisplayDialog("UGS Error",
                            "Maybe, Google Spread Sheet Rules is Invalid or You not make TypeData or other error..",
                            "Hmm.. Ok");
                        break;
                    case TypeParserNotFoundException:
                        EditorUtility.DisplayDialog("UGS Error", e.Message, "Hmm.. Ok");
                        break;
                    case IndexOutOfRangeException:
                        break;
                    default:
                        EditorUtility.DisplayDialog("Please Check Setting!", e.Message, "OK");
                        break;
                }

                errCallback?.Invoke(e);
                Debug.LogError(e);  
            }
        }

        public void CopyFolder(CopyFolderReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<CreateExampleResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }

        public void GetDriveDirectory(GetDriveDirectoryReqModel mdl, Action<System.Exception> errResponse, Action<GetDriveFolderResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<GetDriveFolderResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }

        public void ReadSpreadSheet(ReadSpreadSheetReqModel mdl, Action<System.Exception> errResponse, Action<ReadSpreadSheetResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<ReadSpreadSheetResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }

        public void WriteObject(WriteObjectReqModel mdl, Action<System.Exception> errResponse, Action<WriteObjectResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<WriteObjectResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }

        public void CreateDefaultSheet(CreateDefaultReqModel mdl, Action<System.Exception> errResponse, Action<CreateDefaultSheetResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<CreateDefaultSheetResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }

        public void CopyExample(CopyExampleReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback)
        {
            var url = BaseUrl + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);
            Instance.Get<CreateExampleResult>(url, errResponse, (x) =>
            {
                callback?.Invoke(x);
            });
        }
    }
}
#endif