namespace XTI_App.Abstractions;

public sealed class AuthenticateRequest
{
    public AuthenticateRequest()
        :this(new AppUserName(), "")
    {    
    }

    public AuthenticateRequest(AppUserName userName, string password)
    {
        UserName = userName.DisplayText;
        Password = password;
    }

    public string UserName { get; set; }
    public string Password { get; set; }
}