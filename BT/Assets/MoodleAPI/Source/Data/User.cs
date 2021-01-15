using System;
using UnityEngine;

namespace Moodle {

    [Serializable]
    public class User {

        [SerializeField]
        private uint id;

        public uint Id {
            get { return id; }
        }

        [SerializeField]
        private string username;

        public string Username {
            get { return username; }
        }

        [SerializeField]
        private string fullname;

        public string Fullname {
            get { return fullname; }
        }

        [SerializeField]
        private string email;

        public string Email {
            get { return email; }
        }

        [SerializeField]
        private string department;

        public string Department {
            get { return department; }
        }

        [SerializeField]
        private uint firstaccess;

        public uint FirstAccess {
            get { return firstaccess; }
            set { firstaccess = value; }
        }

        [SerializeField]
        private ulong lastaccess;

        public ulong Lastaccess {
            get { return lastaccess; }
            set { lastaccess = value; }
        }

        [SerializeField]
        private string auth;

        public string Auth {
            get { return auth; }
            set { auth = value; }
        }

        [SerializeField]
        private bool suspended;

        public bool Suspended {
            get { return suspended; }
            set { suspended = value; }
        }

        [SerializeField]
        private bool confirmed;

        public bool Confirmed {
            get { return confirmed; }
            set { confirmed = value; }
        }

        [SerializeField]
        private string lang;

        public string Lang {
            get { return lang; }
            set { lang = value; }
        }

        [SerializeField]
        private string theme;

        public string Theme {
            get { return theme; }
            set { theme = value; }
        }

        [SerializeField]
        private string timezone;

        public string Timezone {
            get { return timezone; }
            set { timezone = value; }
        }

        [SerializeField]
        private uint mailformat;

        public uint MailFormat {
            get { return mailformat; }
            set { mailformat = value; }
        }

        [SerializeField]
        private string description;

        public string Description {
            get { return description; }
            set { description = value; }
        }

        [SerializeField]
        private uint descriptionformat;

        public uint DescriptionFormat {
            get { return descriptionformat; }
            set { descriptionformat = value; }
        }

        [SerializeField]
        private string profileimageurlsmall;

        public string ProfileImageUrlSmall {
            get { return profileimageurlsmall; }
            set { profileimageurlsmall = value; }
        }

        [SerializeField]
        private string profileimageurl;

        public string ProfileImageUrl {
            get { return profileimageurl; }
            set { profileimageurl = value; }
        }

        [SerializeField]
        private UserPreference[] preferences;

        public UserPreference[] Preferences {
            get { return preferences; }
            set { preferences = value; }
        }

        public override string ToString() {
            return base.ToString() + " -> " + string.Format("Id: {0} Username: {1}", Id, Username);
        }

    }

}