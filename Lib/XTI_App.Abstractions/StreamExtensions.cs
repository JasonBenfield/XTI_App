namespace XTI_App.Abstractions;

public static class StreamExtensions
{
    public static byte[] GetBytes(this Stream stream)
    {
        byte[] bytes;
        using (var targetStream = new MemoryStream())
        {
            stream.CopyTo(targetStream);
            bytes = targetStream.ToArray();
        }
        return bytes;
    }
}
