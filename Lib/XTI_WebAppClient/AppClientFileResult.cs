namespace XTI_WebAppClient;

public sealed record AppClientFileResult(byte[] Content, string ContentType, string FileName)
{
    public void WriteToFile(string directory, bool overwriteExisting = false) =>
        WriteToFile(directory, FileName, overwriteExisting);

    public void WriteToFile(string directory, string fileName, bool overwriteExisting = false)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var filePath = Path.Combine(directory, fileName);
        if (File.Exists(filePath))
        {
            if (overwriteExisting)
            {
                File.Delete(filePath);
            }
            else
            {
                throw new Exception($"File '{filePath}' already exists");
            }
        }
        File.WriteAllBytes(filePath, Content);
    }
}