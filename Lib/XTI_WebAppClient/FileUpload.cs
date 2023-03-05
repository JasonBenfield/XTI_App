namespace XTI_WebAppClient;

public sealed record FileUpload
(
    Stream Stream,
    string ContentType,
    string FileName
)
{
    public FileUpload()
        :this(Stream.Null, "", "")
    {
    }
}
