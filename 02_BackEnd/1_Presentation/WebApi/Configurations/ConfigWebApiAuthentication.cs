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
            public void AddConfigAuthentication()
            {
                /************************************************************************************************************
                OBS: Essa autenticação é baseada no token gerado pela aplicação "AutenticacaoTJSP"
                ************************************************************************************************************/

                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
               .AddJwtBearer(x =>
               {
                   x.Authority = $"Tudao_WebApi_{SettingApp.Application.Environment.ToUpper()}";
                   x.RequireHttpsMetadata = false;
                   x.SaveToken = true;
                   x.TokenValidationParameters = new TokenValidationParameters
                   {
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes($"{SettingApp.Application.Environment.ToUpper()}{SettingApp.Authentication.Key}")),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };

                   x.Events = new JwtBearerEvents
                   {
                       OnChallenge = context =>
                       {
                           context.HandleResponse();
                           context.Response.StatusCode = 401;
                           context.Response.ContentType = "application/json";
                           return context.Response.WriteAsync("{\"message\": \"Não Autorizado\"}");
                       },
                       OnForbidden = context =>
                       {
                           context.Response.StatusCode = 403;
                           context.Response.ContentType = "application/json";
                           return context.Response.WriteAsync("{\"message\": \"Acesso Proibido\"}");
                       }
                   };
               });

                //Configuração dos "scope" de acesso e "apolicy" na aplicação
                if (SettingApp.Authentication.Policy.Any())
                {
                    builder.Services.AddAuthorization(options =>
                    {
                        SettingApp.Authentication.PolicyList.ForEach(item => { options.AddPolicy(item.Key, policy => { policy.RequireClaim("scope", item.Value); }); });
                    });
                }
            }
        }
    }
}
