namespace XTI_App
{
    public interface IHashedPasswordFactory
    {
        IHashedPassword Create(string password);
    }
}
