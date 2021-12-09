namespace XTI_App.Abstractions;

public abstract class HashedPassword : IHashedPassword
{
    protected HashedPassword(string password)
    {
        this.password = password;
    }

    private readonly string password;

    public string Value() => Hash(password);

    protected abstract string Hash(string password);

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is string str)
        {
            return Equals(str);
        }
        if (obj is HashedPassword other)
        {
            return other.password == password;
        }
        return false;
    }

    public bool Equals(string? other) => _Equals(password, other ?? "");

    protected abstract bool _Equals(string password, string other);

    public override int GetHashCode() => password.GetHashCode();

}