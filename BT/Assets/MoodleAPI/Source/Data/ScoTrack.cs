using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class ScoTrack {

        [SerializeField]
        private uint attempt;

        public uint Attempt {
            get { return attempt; }
        }

        [SerializeField]
        private ElementData[] tracks;

        public ElementData[] Tracks {
            get { return tracks; }
        }

    }

}