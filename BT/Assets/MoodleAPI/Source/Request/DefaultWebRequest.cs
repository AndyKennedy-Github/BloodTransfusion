using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Moodle {

    public class DefaultWebRequest : IWebRequest {

        private readonly MonoBehaviour context;

        protected UnityWebRequest request;

        protected string error;

        public string Error {
            get { return error; }
        }

        protected string text;

        public string Text {
            get { return text; }
        }

        public DefaultWebRequest(MonoBehaviour context) {
            this.context = context;
        }

        public void Get(string path, ErrorHandler OnError, CompleteHandler OnComplete) {
            context.StartCoroutine(DoGet(path, OnError, OnComplete));
        }

        private IEnumerator DoGet(string path, ErrorHandler OnError, CompleteHandler OnComplete) {
            if (request != null) {
                request.Dispose();
                request = null;
            }

            request = UnityWebRequest.Get(path);

#if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif

            error = request.error;
            text = request.downloadHandler.text;

            if (!string.IsNullOrEmpty(Error)) {
                OnError(Error, Text);
            } else {
                OnComplete(Text);
            }
        }
    }
}