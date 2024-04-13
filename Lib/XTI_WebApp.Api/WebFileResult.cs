namespace XTI_WebApp.Api;

public sealed class WebFileResult
{
    public WebFileResult(Stream fileStream, string contentType, string downloadName)
    {
        FileStream = fileStream;
        ContentType = contentType;
        DownloadName = downloadName;
    }

    public Stream FileStream { get; }
    public string ContentType { get; }
    public string DownloadName { get; }

    public byte[] GetBytes() => FileStream.GetBytes();
}
