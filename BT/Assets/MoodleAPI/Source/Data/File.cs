using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class File {

        [SerializeField]
        private string filename;

        public string FileName {
            get { return filename; }
        }

        [SerializeField]
        private string filepath;

        public string FilePath {
            get { return filepath; }
        }

        [SerializeField]
        private uint filesize;

        public uint FileSize {
            get { return filesize; }
        }

        [SerializeField]
        private string fileurl;

        public string FileUrl {
            get { return fileurl; }
        }

        [SerializeField]
        private uint timemodified;

        public uint TimeModified {
            get { return timemodified; }
        }

        [SerializeField]
        private string mimetype;

        public string MimeType {
            get { return mimetype; }
        }

        [SerializeField]
        private uint isexternalfile;

        public uint IsExternalFile {
            get { return isexternalfile; }
        }

        [SerializeField]
        private string repositorytype;

        public string RepositoryType {
            get { return repositorytype; }
        }

    }

}