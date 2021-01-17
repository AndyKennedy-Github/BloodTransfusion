using System.Collections.Generic;
using System.Globalization;

namespace Moodle {

    public class ScormObjective {

        private readonly string id;
        private readonly float scaledScore;
        private readonly float rawScore;
        private readonly float minScore;
        private readonly float maxScore;
        private readonly string successStatus;
        private readonly string completionStatus;
        private readonly float progressMeasure;
        private readonly string description;

        public ScormObjective(string id, float scaledScore, float rawScore, float minScore, float maxScore, LessonStatus successStatus, LessonStatus completionStatus, float progressMeasure, string description) {
            this.id = id;
            this.scaledScore = scaledScore;
            this.rawScore = rawScore;
            this.minScore = minScore;
            this.maxScore = maxScore;
            this.successStatus = MoodleAPI.LessonStatusToString(successStatus);
            this.completionStatus = MoodleAPI.LessonStatusToString(completionStatus);
            this.progressMeasure = progressMeasure;
            this.description = description;
        }

        public IDictionary<string, string> Build(int index) {
            return new Dictionary<string, string>() {
                { string.Format("cmi.objectives.{0}.id", index), id },
                { string.Format("cmi.objectives.{0}.score.scaled", index), scaledScore.ToString(CultureInfo.InvariantCulture) },
                { string.Format("cmi.objectives.{0}.score.raw", index), rawScore.ToString(CultureInfo.InvariantCulture) },
                { string.Format("cmi.objectives.{0}.score.min", index), minScore.ToString(CultureInfo.InvariantCulture) },
                { string.Format("cmi.objectives.{0}.score.max", index), maxScore.ToString(CultureInfo.InvariantCulture) },
                { string.Format("cmi.objectives.{0}.success_status", index), successStatus },
                { string.Format("cmi.objectives.{0}.completion_status", index), completionStatus },
                { string.Format("cmi.objectives.{0}.progress_measure", index), progressMeasure.ToString(CultureInfo.InvariantCulture) },
                { string.Format("cmi.objectives.{0}.description", index), description }
            };
        }

    }

}
