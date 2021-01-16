using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Moodle {

    public partial class MoodleAPI : MonoBehaviour {

        [Serializable]
        public class ErrorResponse {

            [SerializeField]
            private string error;

            public string Error {
                get { return error; }
            }

            [SerializeField]
            private string errorcode;

            public string ErrorCode {
                get { return errorcode; }
            }

            [SerializeField]
            private string stacktrace;

            public string Stacktrace {
                get { return stacktrace; }
            }

            [SerializeField]
            private string debuginfo;

            public string DebugInfo {
                get { return debuginfo; }
            }

            [SerializeField]
            private string reproductionlink;

            public string ReproductionLink {
                get { return reproductionlink; }
            }

        }

        #region Singleton
        private static MoodleAPI instance;

        public static MoodleAPI Instance {
            get { return instance; }
        }
        #endregion

        private static readonly string Format = "json";

        [SerializeField]
        private string host;

        public string Host {
            get { return host; }
            set { host = value; }
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool endpointLogEnabled;

        public bool EndpointLogEnabled {
            get { return endpointLogEnabled; }
            set { endpointLogEnabled = value; }
        }

        [SerializeField]
        private bool responseLogEnabled;

        public bool ResponseLogEnabled {
            get { return responseLogEnabled; }
            set { responseLogEnabled = value; }
        }
#endif

        private string ApiPath {
            get { return string.Format("{0}/webservice/rest/server.php", Host); }
        }

        private IWebRequestFactory requestFactory;

        public IWebRequestFactory RequestFactory {
            private get { return requestFactory; }
            set { requestFactory = value; }
        }

        private IJsonSerializer serializer = new DefaultJsonSerializer();

        public IJsonSerializer Serializer {
            private get { return serializer; }
            set { serializer = value; }
        }

        public delegate void ErrorHandler(MoodleAPI sender, ErrorResponse error);
        public event ErrorHandler OnError;

        private void Awake() {
            instance = this;

            RequestFactory = new DefaultWebRequestFactory(this);
        }

        private bool IsError(string content) {
            return new Regex("\"error\":\".*\",").IsMatch(content);
        }

        private void Get(string path, Action<string> OnComplete) {
            DoGet(path, (json) => {
                if (OnComplete != null) {
                    OnComplete(json);
                }
            });
        }

        private void Get<T>(string path, Action<T> OnComplete) {
            DoGet(path, (json) => {
                T response = serializer.Deserialize<T>(json);

                if (OnComplete != null) {
                    OnComplete(response);
                }
            });
        }

        private void DoGet(string path, Action<string> OnCompleted) {
#if UNITY_EDITOR
            if (endpointLogEnabled) {
                LogInfo(string.Format("{0}", path));
            }
#endif

            IWebRequest request = RequestFactory.Create();

            request.Get(path,
                (errorMessage, errorJson) => {
                    HandleError(errorMessage, errorJson);
                },
                (json) => {
                    if (!IsError(json)) {
                        HandleComplete(json, OnCompleted);
                    } else {
                        HandleError(string.Empty, json);
                    }
                }
            );
        }

        private void HandleError(string errorMessage, string errorJson) {
#if UNITY_EDITOR
            if (responseLogEnabled) {
                LogError(string.Format("{0} - {1}", errorMessage, errorJson));
            }
#endif

            ErrorResponse response = serializer.Deserialize<ErrorResponse>(errorJson);
            
            if (OnError != null) {
                OnError(this, response);
            } else {
                LogError(string.Format("{0} - {1}", response.ErrorCode, response.Error));
            }
        }

        private void HandleComplete(string json, Action<string> OnComplete) {
#if UNITY_EDITOR
            if (responseLogEnabled) {
                LogInfo(string.Format("Response: {0}", json));
            }
#endif

            OnComplete(json);
        }

        private NameValueCollection Create<T>(string name, IList<T> data) where T : IConvertible {
            NameValueCollection result = new NameValueCollection();

            for (int i = 0; i < data.Count; i++) {
                result.Add(string.Format("{0}[{1}]", name, i), data[i].ToString());
            }

            return result;
        }

        private NameValueCollection Create<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> data) {
            NameValueCollection result = new NameValueCollection();

            int i = 0;
            
            foreach (KeyValuePair<TKey, TValue> pair in data) {
                string elementTitle = string.Format("tracks[{0}][element]", i);
                string valueTitle = string.Format("tracks[{0}][value]", i);

                result.Add(elementTitle, pair.Key.ToString());
                result.Add(valueTitle, pair.Value.ToString());

                i++;
            }

            return result;
        }

        private string BuildAuthUrl(string function, NameValueCollection nameValueCollection) {
            NameValueCollection mergedValueCollection = new NameValueCollection(nameValueCollection) {
                { "wstoken", Token },
                { "wsfunction", function },
                { "moodlewsrestformat", Format }
            };

            return string.Format("{0}?{1}", ApiPath, BuildQuery(mergedValueCollection));
        }

        private string BuildUrl(string basePath, NameValueCollection nameValueCollection) {
            return string.Format("{0}?{1}", basePath, BuildQuery(nameValueCollection));
        }

        private string BuildQuery(NameValueCollection nameValueCollection) {
            return string.Join("&", nameValueCollection.AllKeys.SelectMany(nameValueCollection.GetValues, (k, v) => string.Format("{0}={1}", k, v)).ToArray());
        }

        internal void LogInfo(string text) {
            string message = string.Format("{0}", text);

            Debug.Log(message);
        }

        internal void LogWarning(string text) {
            string message = string.Format("Warning: {0}", text);

            Debug.LogWarning(message);
        }

        internal void LogError(string text) {
            string message = string.Format("Error: {0}", text);

            Debug.LogError(message);
        }

    }

}