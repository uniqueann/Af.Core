using Af.Core.Extensions.Authorizations.Policys;
using Af.Core.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Af.Core.Extensions.Authorizations.Helper
{
    public class JwtToken
    {
        /// <summary>
        /// 获取基于JWT的token
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="permissionRequirement"></param>
        /// <returns></returns>
        public static TokenInfoViewModel BuildJwtToken(Claim[] claims,PermissionRequirement permissionRequirement)
        {
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                issuer: permissionRequirement.Issuer,
                audience: permissionRequirement.Audience,
                claims:claims,
                notBefore:now,
                expires: now.Add(permissionRequirement.Expiration),
                signingCredentials: permissionRequirement.SigningCredentials
            );
            //生成token
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenInfoViewModel { 
                Success = true,
                Token = encodedJwt,
                ExpiresIn = permissionRequirement.Expiration.TotalSeconds,
                TokenType = "Bearer"
            };
        }
    }
}
