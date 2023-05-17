using System.Text;

namespace XTI_WebApp.Extensions;

public sealed record BasicAuthenticationCredentials(string UserName, string Password)
{
    public static BasicAuthenticationCredentials Parse(string auth)
    {
        BasicAuthenticationCredentials cred;
        if (auth.StartsWith("Basic "))
        {
            try
            {
                var base64Bytes = Convert.FromBase64String(auth.Substring(6));
                var base64Str = Encoding.ASCII.GetString(base64Bytes);
                if (base64Str.Contains(':'))
                {
                    var split = base64Str.Split(':');
                    cred = new BasicAuthenticationCredentials(split[0], split[1]);
                }
                else
                {
                    cred = new BasicAuthenticationCredentials("", "");
                }
            }
            catch
            {
                cred = new BasicAuthenticationCredentials("", "");
            }
        }
        else
        {
            cred = new BasicAuthenticationCredentials("", "");
        }
        return cred;
    }
}
