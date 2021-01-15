using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class ElementData {

        [SerializeField]
        private string element;

        public string Element {
            get { return element; }
        }

        [SerializeField]
        private string value;

        public string Value {
            get { return value; }
        }

    }

}