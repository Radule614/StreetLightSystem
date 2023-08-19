using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Common;
/// <summary>
/// Class that contains jwt token configuration.
/// </summary>
public class Bearer
{
    public static IConfiguration? Configuration { get; set; }

    /// <summary>
    /// Delegate which is used by asp.net configuration in order to configure jwt bearer.
    /// </summary>
    public static readonly Action<JwtBearerOptions> BearerOptions = delegate(JwtBearerOptions options)
    {
        if (Configuration == null) return;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = Configuration["JwtSettings:Issuer"],
            ValidAudience = Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"] ?? "")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    };
}
