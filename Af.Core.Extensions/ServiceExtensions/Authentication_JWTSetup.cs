using Af.Core.Common;
using Af.Core.Common.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Extensions
{
    public static class Authentication_JWTSetup
    {
        public static void AddAuthentication_JWTSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var symmetricKeyAsBase64 = AppSecretConfig.AudienceSecretString;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var issuer = Appsettings.app(new string[] { "Audience","Issuer" });
            var audience = Appsettings.app(new string[] { "Audience", "Audience" });

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.Sha256);

            //token验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidIssuer = issuer,//发行人
                ValidateAudience = true,
                ValidAudience = audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true
            };

            // 开启Bearer认证
            services.AddAuthentication(o=> {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //o.DefaultChallengeScheme = nameof();
            });
        }
    }
}
