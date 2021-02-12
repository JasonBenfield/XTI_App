namespace XTI_App.Abstractions
{
    public interface IHashedPasswordFactory
    {
        IHashedPassword Create(string password);
    }
}
