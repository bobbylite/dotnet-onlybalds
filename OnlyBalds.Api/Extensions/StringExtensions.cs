using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

public static class StringExtensions
{
    /// <summary>
    /// Gets the claims principal from a JWT token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="audienceToValidate"></param>
    /// <returns></returns>
    public static async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(this string token, string audienceToValidate)
    {
        var issuer = "https://onlybalds.us.auth0.com/";
        var audience = audienceToValidate;

        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{issuer}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever()
        );

        var config = await configurationManager.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKeys = config.SigningKeys,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

        return principal;
    }
}
