using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Moodle {

    public enum LessonStatus {
        Passed = 0, // 1.2
        Completed = 1, // 1.2 / 2004
        Failed = 2, // 1.2
        Incomplete = 3, // 1.2 / 2004
        Browsed = 4, // 1.2
        NotAttempted = 5, // 1.2 / 2004
        Unknown = 6 // 2004
    }

    public partial class MoodleAPI {

        [Serializable]
        private class ScormResponse {

            [SerializeField]
            private Scorm[] scorms;

            public Scorm[] Scorms {
                get { return scorms; }
            }

        }

        [Serializable]
        private class ScormAttemptsResponse {

            [SerializeField]
            private int attemptscount;

            public int AttemptsCount {
                get { return attemptscount; }
            }

        }

        [Serializable]
        private class ScormScoesResponse {

            [SerializeField]
            private Sco[] scoes;

            public Sco[] Scoes {
                get { return scoes; }
            }

        }

        [Serializable]
        private class ScormScoTrackResponse {

            [SerializeField]
            private ScoTrack data;

            public ScoTrack Data {
                get { return data; }
            }

        }

        [Serializable]
        private class ScormUserDataResponse {

            [SerializeField]
            private ScormUserElementData[] data;

            public ScormUserElementData[] Data {
                get { return data; }
            }

        }

        [Serializable]
        private class InsertScormTracksResponse {

            [SerializeField]
            private uint[] trackids;

            public uint[] TrackIds {
                get { return trackids; }
            }

        }

        [Serializable]
        private class ScormScoLaunchResponse {

            [SerializeField]
            private bool status;

            public bool Status {
                get { return status; }
            }

        }

        [Serializable]
        private class ScormViewResponse {

            [SerializeField]
            private bool status;

            public bool Status {
                get { return status; }
            }

        }

        public delegate void ScormHandler(MoodleAPI sender, Scorm[] scorms);
        public event ScormHandler OnScormsRetrieved;

        public delegate void ScormAttemptHandler(MoodleAPI sender, int count);
        public event ScormAttemptHandler OnScormAttemptsRetrieved;

        public delegate void ScormScoHandler(MoodleAPI sender, Sco[] scoes);
        public event ScormScoHandler OnScormScoesRetrieved;

        public delegate void ScormScoTrackHandler(MoodleAPI sender, ScoTrack scoTrack);
        public event ScormScoTrackHandler OnScormScoTracksRetrieved;

        public delegate void ScormUserDataHandler(MoodleAPI sender, ScormUserElementData[] scoes);
        public event ScormUserDataHandler OnScormUserDataRetrieved;

        public delegate void ScormTracksInserteHandler(MoodleAPI sender, uint[] trackIds);
        public event ScormTracksInserteHandler OnScormTracksInserted;

        public delegate void ScormScoLaunchHandler(MoodleAPI sender, bool result);
        public event ScormScoLaunchHandler OnScormScoLaunched;

        public delegate void ScormViewHandler(MoodleAPI sender, bool result);
        public event ScormViewHandler OnScormViewed;

        /// <summary>
        /// Returns a list of scorm instances the user has access to.
        /// </summary>
        /// <param name="courseIds">Array of course ids.</param>
        public void GetScorms() {
            GetScorms(new List<uint>());
        }

        /// <summary>
        /// Returns a list of scorm instances in a provided set of courses, if no courses are provided then all the scorm instances the user has access to will be returned.
        /// </summary>
        /// <param name="courseIds">Array of course ids.</param>
        public void GetScorms(IList<uint> courseIds) {
            string path = BuildAuthUrl("mod_scorm_get_scorms_by_courses", Create("courseids", courseIds));

            Get<ScormResponse>(path, response => {
                if (OnScormsRetrieved != null) {
                    OnScormsRetrieved(this, response.Scorms);
                }
            });
        }

        /// <summary>
        /// Return the number of attempts done by a user in the given Scorm.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        /// <param name="userId">User id.</param>
        public void GetScormAttemptCount(uint scormId, uint userId) {
            string path = BuildAuthUrl("mod_scorm_get_scorm_attempt_count", new NameValueCollection() {
                { "scormid", scormId.ToString() },
                { "userid", userId.ToString() }
            });

            Get<ScormAttemptsResponse>(path, response => {
                if (OnScormAttemptsRetrieved != null) {
                    OnScormAttemptsRetrieved(this, response.AttemptsCount);
                }
            });
        }

        /// <summary>
        /// Returns a list containing all the scoes data related to the given scorm id.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        public void GetScormScoes(uint scormId) {
            GetScormScoes(scormId, string.Empty);
        }

        /// <summary>
        /// Returns a list containing all the scoes data related to the given scorm id.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        /// <param name="organization">Organization id.</param>
        public void GetScormScoes(uint scormId, string organization) {
            string path = BuildAuthUrl("mod_scorm_get_scorm_scoes", new NameValueCollection() {
                { "scormid", scormId.ToString() },
                { "organization", organization.ToString() }
            });

            Get<ScormScoesResponse>(path, response => {
                if (OnScormScoesRetrieved != null) {
                    OnScormScoesRetrieved(this, response.Scoes);
                }
            });
        }

        /// <summary>
        /// Retrieves SCO tracking data for the given user id and attempt number.
        /// </summary>
        /// <param name="scoId">Sco id.</param>
        /// <param name="userId">User id.</param>
        public void GetScormScoTracks(uint scoId, uint userId) {
            GetScormScoTracks(scoId, userId, 0);
        }

        /// <summary>
        /// Retrieves SCO tracking data for the given user id and attempt number.
        /// </summary>
        /// <param name="scoId">Sco id.</param>
        /// <param name="userId">User id.</param>
        /// <param name="attempt">Attempt number (0 for last attempt).</param>
        public void GetScormScoTracks(uint scoId, uint userId, int attempt) {
            string path = BuildAuthUrl("mod_scorm_get_scorm_sco_tracks", new NameValueCollection() {
                { "scoid", scoId.ToString() },
                { "userid", userId.ToString() },
                { "attempt", attempt.ToString() }
            });

            Get<ScormScoTrackResponse>(path, response => {
                if (OnScormScoTracksRetrieved != null) {
                    OnScormScoTracksRetrieved(this, response.Data);
                }
            });
        }

        /// <summary>
        /// Retrieves user tracking and Sco data and default Scorm values.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        /// <param name="attempt">Attempt number.</param>
        public void GetScormUserData(uint scormId, int attempt) {
            string path = BuildAuthUrl("mod_scorm_get_scorm_user_data", new NameValueCollection() {
                { "scormid", scormId.ToString() },
                { "attempt", attempt.ToString() }
            });

            Get<ScormUserDataResponse>(path, response => {
                if (OnScormUserDataRetrieved != null) {
                    OnScormUserDataRetrieved(this, response.Data);
                }
            });
        }

        /// <summary>
        /// Saves a scorm tracking record. It will overwrite any existing tracking data for this attempt. Validation should be performed before running the function to ensure the user will not lose any existing attempt data.
        /// </summary>
        /// <param name="scoId">Sco id.</param>
        /// <param name="attempt">Attempt number.</param>
        /// <param name="data">Data to record.</param>
        public void InsertScormTracks(uint scoId, uint attempt, ICollection<KeyValuePair<string, string>> data) {
            string path = BuildAuthUrl("mod_scorm_insert_scorm_tracks", new NameValueCollection(Create(data)) {
                { "scoid", scoId.ToString() },
                { "attempt", attempt.ToString() }
            });

            Get<InsertScormTracksResponse>(path, response => {
                if (OnScormTracksInserted != null) {
                    OnScormTracksInserted(this, response.TrackIds);
                }
            });
        }

        /// <summary>
        /// Trigger the SCO launched event.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        public void ScormLaunchSco(uint scormId) {
            ScormLaunchSco(scormId, 0);
        }

        /// <summary>
        /// Trigger the SCO launched event.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        /// <param name="scoId">Sco id (empty for launching the first Sco).</param>
        public void ScormLaunchSco(uint scormId, uint scoId) {
            string path = BuildAuthUrl("mod_scorm_launch_sco", new NameValueCollection() {
                { "scormid", scormId.ToString() },
                { "scoid", scoId.ToString() }
            });

            Get<ScormScoLaunchResponse>(path, response => {
                if (OnScormScoLaunched != null) {
                    OnScormScoLaunched(this, response.Status);
                }
            });
        }

        /// <summary>
        /// Trigger the course module viewed event.
        /// </summary>
        /// <param name="scormId">Scorm instance id.</param>
        public void ScormView(uint scormId) {
            string path = BuildAuthUrl("mod_scorm_view_scorm", new NameValueCollection() {
                { "scormid", scormId.ToString() }
            });

            Get<ScormViewResponse>(path, response => {
                if (OnScormViewed != null) {
                    OnScormViewed(this, response.Status);
                }
            });
        }

        public static string LessonStatusToString(LessonStatus lessonStatus) {
            string status = string.Empty;

            if (lessonStatus == LessonStatus.Passed) {
                status = "passed";
            } else if (lessonStatus == LessonStatus.Completed) {
                status = "completed";
            } else if (lessonStatus == LessonStatus.Failed) {
                status = "failed";
            } else if (lessonStatus == LessonStatus.Incomplete) {
                status = "incomplete";
            } else if (lessonStatus == LessonStatus.Browsed) {
                status = "browsed";
            } else if (lessonStatus == LessonStatus.NotAttempted) {
                status = "not attempted";
            } else if (lessonStatus == LessonStatus.Unknown) {
                status = "unknown";
            } else {
                throw new UnityException("[Moodle] No valid lessonStatus value: '" + lessonStatus + "'");
            }

            return status;
        }

    }

}