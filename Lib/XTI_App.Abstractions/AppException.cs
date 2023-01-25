namespace XTI_App.Abstractions;

public class AppException : Exception
{
    public AppException(string message, Exception? innerException = null)
        : this(message, message, innerException)
    {
    }

    public AppException(string message, string displayMessage, Exception? innerException = null)
        : base(message, innerException)
    {
        DisplayMessage = displayMessage;
    }

    public string DisplayMessage { get; }
}