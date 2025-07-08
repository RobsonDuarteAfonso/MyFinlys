using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyFinlys.Api.JWT;

namespace MyFinlys.Api.Extensions
{
    public static class JwtServiceExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // Configuração de JwtSettings
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            var jwt = config.GetSection("JwtSettings").Get<JwtSettings>()
                      ?? throw new InvalidOperationException("JWT settings missing");
            var key = Encoding.UTF8.GetBytes(jwt.Key);

            // Autenticação JWT
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata   = true;
                    options.SaveToken              = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidIssuer              = jwt.Issuer,
                        ValidateAudience         = true,
                        ValidAudience            = jwt.Audience,
                        ValidateLifetime         = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(key),
                        ValidateIssuerSigningKey = true,
                        ClockSkew                = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();
            return services;
        }
    }
}
