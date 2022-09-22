namespace XTI_WebAppClient;

public sealed record FileUpload
(
    Stream Stream,
    string ContentType,
    string ContentDisposition,
    string Name,
    string FileName
)
{
    public FileUpload()
        :this(Stream.Null, "", "", "", "")
    {
    }
}
