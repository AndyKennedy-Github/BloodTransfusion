using System;
using UnityEngine;

namespace Moodle {

    public class DefaultJsonSerializer : IJsonSerializer {

        [Serializable]
        public class Wrapper<T> {

            [SerializeField]
            private T[] array;

            public T[] Array {
                get { return array; }
            }
        }

        public string Serialize(object obj) {
            return JsonUtility.ToJson(obj);
        }

        public T Deserialize<T>(string json) {
            return JsonUtility.FromJson<T>(json);
        }

        public T[] DeserializeArray<T>(string json) {
            string newJson = string.Format("{{ \"array\": {0}}}", json);

            Wrapper<T> wrapper = Deserialize<Wrapper<T>>(newJson);

            return wrapper.Array;
        }

    }

}