using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace XTI_WebApp.Extensions;

public sealed class JwtAuthTicketFormat : ISecureDataFormat<AuthenticationTicket>
{
    private const string Algorithm = SecurityAlgorithms.HmacSha256;
    private readonly TokenValidationParameters validationParameters;
    private readonly IDataSerializer<AuthenticationTicket> ticketSerializer;
    private readonly IDataProtector dataProtector;

    public JwtAuthTicketFormat
    (
        TokenValidationParameters validationParameters,
        IDataSerializer<AuthenticationTicket> ticketSerializer,
        IDataProtector dataProtector
    )
    {
        this.validationParameters = validationParameters;
        this.ticketSerializer = ticketSerializer;
        this.dataProtector = dataProtector;
    }

    public AuthenticationTicket? Unprotect(string? protectedText)
        => Unprotect(protectedText, null);

    public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
    {
        AuthenticationTicket? authTicket;
        try
        {
            var unprotectedBytes = dataProtector.Unprotect
            (
                Base64UrlTextEncoder.Decode(protectedText ?? "")
            );
            authTicket = ticketSerializer.Deserialize(unprotectedBytes);
            var embeddedJwt = authTicket?.Properties?.GetTokenValue("Jwt");
            new JwtSecurityTokenHandler().ValidateToken(embeddedJwt, validationParameters, out var token);
            if (!(token is JwtSecurityToken jwt))
            {
                throw new SecurityTokenValidationException("JWT token was found to be invalid");
            }
            if (!jwt.Header.Alg.Equals(Algorithm, StringComparison.Ordinal))
            {
                throw new ArgumentException($"Algorithm must be '{Algorithm}'");
            }
        }
        catch (Exception)
        {
            authTicket = null;
        }
        return authTicket;
    }

    public string Protect(AuthenticationTicket data) => Protect(data, null);

    public string Protect(AuthenticationTicket data, string? purpose)
    {
        var serializedTicket = ticketSerializer.Serialize(data);
        return Base64UrlTextEncoder.Encode(dataProtector.Protect(serializedTicket));
    }
}