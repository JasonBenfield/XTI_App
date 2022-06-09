using System.Net;

namespace XTI_WebAppClient;

public sealed class AppClientException : Exception
{
    public AppClientException(string url, HttpStatusCode statusCode, string responseContent, IEnumerable<ErrorModel> errors)
        : base(formatErrorMessage(url, statusCode, responseContent, errors))
    {
        Url = url;
        StatusCode = statusCode;
        ResponseContent = responseContent;
        Errors = errors;
    }

    private static string formatErrorMessage(string url, HttpStatusCode statusCode, string responseContent, IEnumerable<ErrorModel> errors)
    {
        var joinedErrors = string.Join("\r\n", errors.Select(e => e.Format()));
        return $"{url}\r\n{statusCode}\r\n{joinedErrors}\r\n{responseContent}";
    }

    public string Url { get; }
    public HttpStatusCode StatusCode { get; }
    public string ResponseContent { get; }
    public IEnumerable<ErrorModel> Errors { get; }
}