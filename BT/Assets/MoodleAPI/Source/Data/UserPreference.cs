using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class UserPreference {

        [SerializeField]
        private string name;

        public string Name {
            get { return name; }
        }

        [SerializeField]
        private string value;

        public string Value {
            get { return value; }
        }

    }

}