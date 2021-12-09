using XTI_App.Abstractions;

namespace XTI_App.Fakes;

public sealed class FakeHashedPasswordFactory : IHashedPasswordFactory
{
    public IHashedPassword Create(string password) => new FakeHashedPassword(password);
}