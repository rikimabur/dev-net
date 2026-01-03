using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace multiple_authentication_schemes;
public sealed class JwtBearerOptionsSetup<TOptions>
    : IConfigureNamedOptions<JwtBearerOptions>
    where TOptions : class
{
    private readonly TOptions _options;
    private readonly string _schemeName;

    public JwtBearerOptionsSetup(
        IOptions<TOptions> options,
        string schemeName)
    {
        _options = options.Value;
        _schemeName = schemeName;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != _schemeName)
            return;

        dynamic jwt = _options;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Key))
        };
    }

    public void Configure(JwtBearerOptions options)
        => Configure(Options.DefaultName, options);
}