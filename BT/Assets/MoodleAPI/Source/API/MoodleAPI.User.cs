using System;
using System.Collections.Specialized;
using UnityEngine;

namespace Moodle {

    public partial class MoodleAPI {

        [Serializable]
        private class LoginResponse {

            [SerializeField]
            private string token;

            public string Token {
                get { return token; }
            }
        }

        public string Token { get; private set; }

        public delegate void TokenHandler(MoodleAPI sender);
        public event TokenHandler OnTokenRetrieved;

        public delegate void UserDetailsHandler(MoodleAPI sender, User[] users);
        public event UserDetailsHandler OnUserDetailsRetrieved;

        public void GetToken(string username, string password) {
            GetToken(username, password, "moodle_mobile_app");
        }

        public void GetToken(string username, string password, string service) {
            string path = string.Format("{0}/login/token.php", Host);

            path = BuildUrl(path, new NameValueCollection() {
                { "username", username },
                { "password", password },
                { "service", service }
            });

            Get<LoginResponse>(path, response => {
                Token = response.Token;

                if (OnTokenRetrieved != null) {
                    OnTokenRetrieved(this);
                }
            });
        }

        public void GetUser(string field, string value) {
            string path = BuildAuthUrl("core_user_get_users_by_field", new NameValueCollection() {
                { "field", field },
                { "values[0]", value }
            });

            Get(path, json => {
                User[] response = serializer.DeserializeArray<User>(json);

                if (OnUserDetailsRetrieved != null) {
                    OnUserDetailsRetrieved(this, response);
                }
            });
        }

    }

}