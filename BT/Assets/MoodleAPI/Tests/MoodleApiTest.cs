using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace Moodle {

    public partial class MoodleApiTest {

        private const string Host = "http://localhost:4001";
        private const string User = "rest_test";
        private const string Password = "$_M00dle_$";
        private const uint ScormId = 1;
        private const uint ScoId = 2;

        private class StateData {

            public bool Finished { get; set; }
        }

        private delegate void PreCallback(MoodleAPI rest, StateData data);
        private delegate void PostCallback(MoodleAPI rest);

        private IEnumerator AuthExec(PreCallback preCallback) {
            yield return Exec(
                (rest, data) => {
                    rest.OnTokenRetrieved += sender => {
                        preCallback(rest, data);
                    };
                    rest.GetToken(User, Password);
                }
            );
        }

        private IEnumerator Exec(PreCallback preCallback) {
            yield return Exec(preCallback, null);
        }

        private IEnumerator Exec(PreCallback preCallback, PostCallback postCallback) {
            GameObject go = new GameObject();

            MoodleAPI rest = go.AddComponent<MoodleAPI>();
            rest.Host = Host;
            rest.EndpointLogEnabled = true;
            rest.ResponseLogEnabled = true;
            rest.OnError += (sender, error) => {
                Assert.Fail(error.Error);
            };

            StateData data = new StateData();

            preCallback(rest, data);

            while (!data.Finished) {
                yield return null;
            }

            if (postCallback != null) {
                postCallback(rest);
            }

            Object.DestroyImmediate(go);
        }

    }

}