using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Settings;
using System.Text;

namespace WebApi.Configurations
{
    public static class ConfigWebApiAuthentication
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddAuthentication()
            {
                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Authority = $"App_{SettingApp.Application.Environment}";
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes($"{SettingApp.Application.Environment}{SettingApp.Parameters.KeyToken}")),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

                //Configuração dos "scope" de acesso e "apolicy" na aplicação
                if (SettingApp.Parameters.Policies.Count != 0)
                {
                    builder.Services.AddAuthorization(options =>
                    {
                        SettingApp.Parameters.Policies.ForEach(item => { options.AddPolicy(item.Name, policy => { policy.RequireClaim("scope", item.Scope); }); });
                    });
                }
            }
        }
    }
}
