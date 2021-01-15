using System;
using System.Collections.Generic;
using System.Linq;

namespace Moodle {

    public class ScormDataBuilder {

        private readonly IDictionary<string, string> result = new Dictionary<string, string>();

        private readonly IList<ScormObjective> objectives = new List<ScormObjective>();

        public ScormDataBuilder SetLessonLocation(string value) {
            result["cmi.lesson_location"] = value;

            return this;
        }

        public ScormDataBuilder SetLessonStatus(string value) {
            result["cmi.core.lesson_status"] = value;

            return this;
        }

        public ScormDataBuilder SetMinScore(float score) {
            result["cmi.core.score.min"] = score.ToString();

            return this;
        }

        public ScormDataBuilder SetMaxScore(float score) {
            result["cmi.core.score.max"] = score.ToString();

            return this;
        }

        public ScormDataBuilder SetRawScore(float score) {
            result["cmi.core.score.raw"] = score.ToString();

            return this;
        }

        public ScormDataBuilder SetSessionTime(TimeSpan timeSpan) {
            string value = string.Format("PT{0}H{1}M{2}S", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            return SetSessionTime(value);
        }

        public ScormDataBuilder SetSessionTime(string value) {
            result["cmi.core.session_time"] = value;

            return this;
        }

        public ScormDataBuilder SetSuspendData(string data) {
            result["cmi.suspend_data"] = data;

            return this;
        }

        public ScormDataBuilder AddObjective(ScormObjective objective) {
            objectives.Add(objective);

            return this;
        }

        public IDictionary<string, string> Build() {
            Dictionary<string, string> data = new Dictionary<string, string>(result);

            for (int i = 0; i < objectives.Count; i++) {
                IDictionary<string, string> objective = objectives[i].Build(i);

                objective.ToList().ForEach(x => data[x.Key] = x.Value);
            }

            return data;
        }

    }

}
