using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class Md5HashedPasswordFactory : IHashedPasswordFactory
    {
        public IHashedPassword Create(string password) => new Md5HashedPassword(password);
    }
}
