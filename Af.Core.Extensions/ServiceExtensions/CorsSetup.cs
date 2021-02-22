using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Af.Core.Extensions
{
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddCors(c =>
            {
                if (!Appsettings.app(new string[] { "Startup", "Cors", "EnableAllIPs" }).ObjToBool())
                {
                    c.AddPolicy(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .WithOrigins(Appsettings.app(new string[] { "Startup", "Cors", "IPs" }).Split(','))
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                        });

                }
                else
                {
                    // 允许任意跨域请求
                    c.AddPolicy(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowCredentials();
                        });
                }
            });
        }
    }
}
