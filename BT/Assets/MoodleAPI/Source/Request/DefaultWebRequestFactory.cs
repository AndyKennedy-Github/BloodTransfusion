using UnityEngine;

namespace Moodle {

    public class DefaultWebRequestFactory : IWebRequestFactory {

        private readonly MonoBehaviour context;

        public DefaultWebRequestFactory(MonoBehaviour context) {
            this.context = context;
        }

        public IWebRequest Create() {
            return new DefaultWebRequest(context);
        }
    }

}
