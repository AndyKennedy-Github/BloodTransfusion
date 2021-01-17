using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class Sco {

        [SerializeField]
        private uint id;

        public uint Id {
            get { return id; }
        }

        [SerializeField]
        private uint scorm;

        public uint Scorm {
            get { return scorm; }
        }

        [SerializeField]
        private string manifest;

        public string Manifest {
            get { return manifest; }
        }

        [SerializeField]
        private string organization;

        public string Organization {
            get { return organization; }
        }

        [SerializeField]
        private string parent;

        public string Parent {
            get { return parent; }
        }

        [SerializeField]
        private string identifier;

        public string Identifier {
            get { return identifier; }
        }

        [SerializeField]
        private string launch;

        public string Launch {
            get { return launch; }
        }

        [SerializeField]
        private string scormtype;

        public string ScormType {
            get { return scormtype; }
        }

        [SerializeField]
        private string title;

        public string Title {
            get { return title; }
        }

        [SerializeField]
        private uint sortorder;

        public uint SortOrder {
            get { return sortorder; }
        }

        [SerializeField]
        private ScoExtraData[] extradata;

        public ScoExtraData[] ExtraData {
            get { return extradata; }
        }

    }

}
