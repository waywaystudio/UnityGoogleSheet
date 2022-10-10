using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class UnityPlayerWebRequest : MonoBehaviour, IHttpProtocol
    {
        public bool reqProcessing;
        public static UnityPlayerWebRequest Instance
        {
            get
            {
                if (instance == null)
                {
                    var data = new GameObject().AddComponent<UnityPlayerWebRequest>();
                    instance = data;
                    data.gameObject.name = "UnityPlayerWebRequest";
                }
                return instance;
            }
        }
        private static UnityPlayerWebRequest instance;

        public string baseURL => UgsConfig.Instance.GoogleScriptUrl;

        private void Awake()
        {
            //singleton
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
        }
        private IEnumerator Get<T>(string uri, Action<System.Exception> OnError, Action<T> callback) where T : Response
        {
            using var webRequest = UnityWebRequest.Get(uri);
            webRequest.timeout = 60;
            yield return webRequest.SendWebRequest();
            reqProcessing = false;
            
            if (webRequest.error == null)
            {
                var deserialize = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                if (!deserialize.hasError())
                {
                    callback?.Invoke(deserialize);
                } 
                else
                { 
                    OnError(new System.Exception("Response Is Null, Error Json => " + webRequest.downloadHandler.text));
                }
            }
            else
            {
                reqProcessing = false;
                OnError(new System.Exception(webRequest.error));
            }
        }


        private IEnumerator Post<T>(string json, Action<System.Exception> OnError, Action<T> callback) where T : Response
        { 
            var jo = JObject.Parse(json);
            jo.Add("password", UgsConfig.Instance.Password);
            json = jo.ToString();
            var request = new UnityWebRequest(baseURL, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 60;
            yield return request.SendWebRequest();
            reqProcessing = false;
            if (request.error == null)
            { 
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                if (data != null && data.hasError())
                {
                    OnError(new UGSWebError(data.error.message));
                }
                else
                { 
                    callback?.Invoke(data);
                }
            }
            else
            {
                OnError?.Invoke(new System.Exception(request.error));
            }
        }

        public void GetDriveDirectory(GetDriveDirectoryReqModel mdl, Action<System.Exception> errResponse, Action<GetDriveFolderResult> callback)
        {
            if (reqProcessing)
            {
                Debug.Log("already requested! wait response!");
                return;
            }

            reqProcessing = true;

            var url = baseURL + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);

            StartCoroutine(Get<GetDriveFolderResult>(url, errResponse, (x) =>
            {
                if (x.hasError())
                    errResponse?.Invoke(new UGSWebError(x.error.message));
                else
                {
                    callback?.Invoke(x);
                }
            }));
        }

        public void ReadSpreadSheet(ReadSpreadSheetReqModel mdl, Action<System.Exception> errResponse, Action<ReadSpreadSheetResult> callback)
        {
            if (reqProcessing)
            {
                Debug.Log("already requested! wait response!");
                return;
            }

            reqProcessing = true;

            var url = baseURL + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password); 
            StartCoroutine(Get<ReadSpreadSheetResult>(url, errResponse, (x) =>
            {
                if (x.hasError())
                    errResponse?.Invoke(new UGSWebError(x.error.message));
                else
                {
                    callback?.Invoke(x);
                }
            }));
        }

        public void WriteObject(WriteObjectReqModel mdl, Action<System.Exception> errResponse, Action<WriteObjectResult> callback)
        {
            if (reqProcessing)
            {
                Debug.Log("already requested! wait response!");
                return;
            }

            reqProcessing = true;
            
            StartCoroutine(Post<WriteObjectResult>(Newtonsoft.Json.JsonConvert.SerializeObject(mdl), errResponse, (x) =>
            {
                if (x.hasError())
                    errResponse?.Invoke(new UGSWebError(x.error.message));
                else
                {
                    callback?.Invoke(x);
                }
            }));
        }

        public void CreateDefaultSheet(CreateDefaultReqModel mdl, Action<System.Exception> errResponse, Action<CreateDefaultSheetResult> callback)
        {
            if (reqProcessing)
            {
                Debug.Log("already requested! wait response!");
                return;
            }

            reqProcessing = true;

            var url = baseURL + UgsUtility.ToQueryString(mdl, UgsConfig.Instance.Password);

            StartCoroutine(Get<CreateDefaultSheetResult>(url, errResponse, (x) =>
            {
                if (x.hasError())
                    errResponse?.Invoke(new UGSWebError(x.error.message));
                else
                {
                    callback?.Invoke(x);
                }
            }));
        }

        public void CopyExample(CopyExampleReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback)
        {
            if (reqProcessing)
            {
                Debug.Log("already requested! wait response!");
                return;
            }

            reqProcessing = true;

            var url = baseURL + UgsUtility.ToQueryString(mdl,UgsConfig.Instance.Password);

             StartCoroutine(Get<CreateExampleResult>(url, errResponse, (x) =>
             {
                 if (x.hasError()) 
                     errResponse?.Invoke(new UGSWebError(x.error.message)); 
                 else
                 {
                     callback?.Invoke(x);
                 }
             }));
        }

        public void CopyFolder(CopyFolderReqModel mdl, Action<System.Exception> errResponse, Action<CreateExampleResult> callback)
        {
            throw new NotImplementedException();
        }
    }
}