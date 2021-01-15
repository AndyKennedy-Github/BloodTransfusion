using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace Moodle {

    public partial class MoodleApiTest {

        [UnityTest]
        public IEnumerator GetScorms() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnScormsRetrieved += (sender, scorms) => {
                        Assert.NotNull(scorms);
                        Assert.IsNotEmpty(scorms);

                        data.Finished = true;
                    };
                    rest.GetScorms();
                }
            );
        }

        [UnityTest]
        public IEnumerator GetScormAttemptCount() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnUserDetailsRetrieved += (sender, users) => {
                        rest.OnScormAttemptsRetrieved += (sender2, count) => {
                            Assert.Greater(count, 0);

                            data.Finished = true;
                        };
                        rest.GetScormAttemptCount(ScormId, users[0].Id);
                    };
                    rest.GetUser("username", User);
                }
            );
        }

        [UnityTest]
        public IEnumerator GetScormScoes() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnScormScoesRetrieved += (sender, scoes) => {
                        Assert.NotNull(scoes);
                        Assert.IsNotEmpty(scoes);

                        data.Finished = true;
                    };
                    rest.GetScormScoes(ScormId, string.Empty);
                }
            );
        }

        [UnityTest]
        public IEnumerator GetScormScoTracks() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnUserDetailsRetrieved += (sender, users) => {
                        rest.OnScormScoTracksRetrieved += (sender2, scoTrack) => {
                            Assert.Greater(scoTrack.Attempt, 0);
                            Assert.NotNull(scoTrack.Tracks);
                            Assert.IsNotEmpty(scoTrack.Tracks);

                            data.Finished = true;
                        };
                        rest.GetScormScoTracks(ScoId, users[0].Id);
                    };
                    rest.GetUser("username", User);
                }
            );
        }

        [UnityTest]
        public IEnumerator GetScormUserData() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnScormUserDataRetrieved += (sender, scoes) => {
                        Assert.NotNull(scoes);
                        Assert.IsNotEmpty(scoes);

                        data.Finished = true;
                    };
                    rest.GetScormUserData(ScormId, 4);
                }
            );
        }

        [UnityTest]
        public IEnumerator InsertScormTracksSimple() {
            yield return AuthExec(
                (rest, data) => {
                    IDictionary<string, string> result = new ScormDataBuilder()
                        .SetLessonLocation("7")
                        .SetLessonStatus("completed")
                        .SetMinScore(0)
                        .SetMaxScore(100)
                        .SetRawScore(75)
                        .SetSuspendData("test data")
                        .SetSessionTime(new TimeSpan(1, 10, 25))
                        .AddObjective(new ScormObjective("objective 0", 0.5f, 1.5f, 0, 3, LessonStatus.Passed, LessonStatus.Completed, 0.1f, "objective 0 description"))
                        .Build();

                    rest.OnScormTracksInserted += (sender, trackIds) => {
                        Assert.NotNull(trackIds);
                        Assert.IsNotEmpty(trackIds);

                        data.Finished = true;
                    };
                    rest.InsertScormTracks(ScoId, 4, result);
                }
            );
        }

        [UnityTest]
        public IEnumerator InsertScormTracksAdvanced() {
            yield return AuthExec(
                (rest, data) => {
                    IDictionary<string, string> result = new Dictionary<string, string>() {
                        { "cmi.core.lesson_location", "7" },
                        { "cmi.core.lesson_status", "completed" },
                        { "cmi.core.score.min", "0" },
                        { "cmi.core.score.max", "100" },
                        { "cmi.core.score.raw", "75" },
                        { "cmi.suspend_data", "test data" },
                        { "cmi.core.session_time", "PT1H10M25S" },
                        { "cmi.objectives.0.id", "objective 0" },
                        { "cmi.objectives.0.score.scaled", "0.5" },
                        { "cmi.objectives.0.score.raw", "1.5" },
                        { "cmi.objectives.0.score.min", "0" },
                        { "cmi.objectives.0.score.max", "3" },
                        { "cmi.objectives.0.success_status", "passed" },
                        { "cmi.objectives.0.completion_status", "completed" },
                        { "cmi.objectives.0.progress_measure", "0.1" },
                        { "cmi.objectives.0.description", "objective 0 description" }
                    };

                    rest.OnScormTracksInserted += (sender, trackIds) => {
                        Assert.NotNull(trackIds);
                        Assert.IsNotEmpty(trackIds);

                        data.Finished = true;
                    };
                    rest.InsertScormTracks(ScoId, 4, result);
                }
            );
        }

        [UnityTest]
        public IEnumerator ScomLaunchSco() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnScormScoLaunched += (sender, result) => {
                        Assert.IsTrue(result);

                        data.Finished = true;
                    };
                    rest.ScormLaunchSco(ScormId);
                }
            );
        }

        [UnityTest]
        public IEnumerator ScormView() {
            yield return AuthExec(
                (rest, data) => {
                    rest.OnScormViewed += (sender, result) => {
                        Assert.IsTrue(result);

                        data.Finished = true;
                    };
                    rest.ScormView(ScormId);
                }
            );
        }

    }

}