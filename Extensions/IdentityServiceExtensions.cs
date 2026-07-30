using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ToDoApi.Extensions;

public static class IdentityServiceExtensions
{
    private const int MinKeyBytes = 32;

    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        var keyString = config["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(keyString))
            throw new InvalidOperationException(
                "JWT key is missing. Provide 'Jwt:Key' via environment variables or user secrets — never in appsettings.json.");

        if (Encoding.UTF8.GetByteCount(keyString) < MinKeyBytes)
            throw new InvalidOperationException(
                $"JWT key is too short. HMAC-SHA256 requires at least {MinKeyBytes} bytes.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Disable the default claim-type remapping that maps
                //   "sub"         → ClaimTypes.NameIdentifier
                //   "unique_name" → ClaimTypes.Name
                //   etc.
                // With this set to false, claims keep their original JWT names,
                // so FindFirst(JwtRegisteredClaimNames.Sub) works as expected.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer      = config["Jwt:Issuer"],
                    ValidAudience    = config["Jwt:Audience"],
                    IssuerSigningKey = signingKey,

                    // Tell ASP.NET Core which raw JWT claim to use for
                    // User.Identity.Name and [Authorize(Roles=...)] checks.
                    NameClaimType = JwtRegisteredClaimNames.UniqueName, // "unique_name"
                    RoleClaimType = "Role",

                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }
}
