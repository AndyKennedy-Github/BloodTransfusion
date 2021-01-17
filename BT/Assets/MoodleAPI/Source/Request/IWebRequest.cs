namespace Moodle {

    public delegate void ErrorHandler(string message, string json);
    public delegate void CompleteHandler(string text);

    public interface IWebRequest {

        string Error { get; }

        string Text { get; }

        void Get(string path, ErrorHandler OnError, CompleteHandler OnComplete);
    }
}