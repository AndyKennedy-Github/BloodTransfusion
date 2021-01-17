using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class ScormUserElementData {

        [SerializeField]
        private uint scoid;

        public uint ScoId {
            get { return scoid; }
        }

        [SerializeField]
        private ElementData[] userdata;

        public ElementData[] UserData {
            get { return userdata; }
        }

        [SerializeField]
        private ElementData[] defaultdata;

        public ElementData[] DefaultData {
            get { return defaultdata; }
        }

    }

}