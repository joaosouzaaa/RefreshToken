using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RefreshTokenAuthentication.API.Constants;
using RefreshTokenAuthentication.API.Events;
using System.Text;

namespace RefreshTokenAuthentication.API.DependencyInjection;

internal static class AuthenticationDependencyInjection
{
    internal static void AddAuthenticationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        var token = configuration.GetSection(OptionsConstants.TokenSection).Get<Options.TokenOptions>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Key)),
                ValidateIssuer = true,
                ValidIssuer = token.Issuer,
                ValidateAudience = true,
                ValidAudience = token.Audience,
                ClockSkew = TimeSpan.Zero
            };
            options.EventsType = typeof(TokenRevokedBearerEvents);
        })
       .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(IdentityConstants.ApplicationScheme);

        services.AddScoped<TokenRevokedBearerEvents>();
    }
}
