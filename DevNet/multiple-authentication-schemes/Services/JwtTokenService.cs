using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace multiple_authentication_schemes.Services;
public interface IJwtTokenService
{
    string CreatePublicToken(string username);
    string CreateInternalToken(string username);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtPublicOptions _public;
    private readonly JwtInternalOptions _internal;

    public JwtTokenService(
        IOptions<JwtPublicOptions> publicOptions,
        IOptions<JwtInternalOptions> internalOptions)
    {
        _public = publicOptions.Value;
        _internal = internalOptions.Value;
    }

    public string CreatePublicToken(string username)
        => CreateToken(_public, username);

    public string CreateInternalToken(string username)
        => CreateToken(_internal, username);

    private static string CreateToken(dynamic options, string username)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Key));

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: new[]
            {
                new Claim(ClaimTypes.Name, username)
            },
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}