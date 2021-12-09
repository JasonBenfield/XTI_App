namespace XTI_App.Abstractions;

public interface IHashedPassword : IEquatable<string>
{
    string Value();
}