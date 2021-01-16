namespace Moodle {

    public interface IJsonSerializer {

        string Serialize(object obj);

        T Deserialize<T>(string json);

        T[] DeserializeArray<T>(string json);

    }

}