using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Moodle {

    public partial class MoodleApiTest {

        [UnityTest]
        public IEnumerator GetToken() {
            yield return Exec(
                (rest, data) => {
                    rest.OnTokenRetrieved += sender => {
                        Assert.NotNull(sender.Token);

                        data.Finished = true;
                    };
                    rest.GetToken(User, Password);
                }
            );
        }

        [UnityTest]
        public IEnumerator GetUser() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnUserDetailsRetrieved += (sender, response) => {
                        Assert.IsNotEmpty(response);
                        Assert.Greater(response[0].Id, 0);

                        data.Finished = true;
                    };
                    rest.GetUser("username", User);
                }
            );
        }

    }

}