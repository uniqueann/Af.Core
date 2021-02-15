using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Extensions.Authorizations.Helper;
using Af.Core.Extensions.Authorizations.Policys;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        readonly ISysUserInfoServices _sysUserInfoServices;
        readonly IUserRoleServices _userRoleServices;
        readonly IRoleServices _roleServices;
        readonly PermissionRequirement _permissionRequirement;
        readonly IRoleModulePermissionServices _roleModulePermissionServices;

        public AuthController(ISysUserInfoServices sysUserInfoServices, IUserRoleServices userRoleServices,
                              IRoleServices roleServices, PermissionRequirement permissionRequirement,
                              IRoleModulePermissionServices roleModulePermissionServices)
        {
            _sysUserInfoServices = sysUserInfoServices;
            _userRoleServices = userRoleServices;
            _roleServices = roleServices;
            _permissionRequirement = permissionRequirement;
            _roleModulePermissionServices = roleModulePermissionServices;
        }




        /// <summary>
        /// 登录接口：随便输入字符，获取token，然后添加 Authoritarian
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getJwtTokenStr")]
        public async Task<object> GetJwtTokenStr(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            //这里直接写死了


            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new JsonResult(new
                {
                    Status = false,
                    message = "用户名或密码不能为空"
                });
            }

            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = 1;
            tokenModel.Role = "Admin";

            jwtStr = JwtHelper.IssueJWT(tokenModel);
            suc = true;


            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }

        /// <summary>
        /// 登录接口 验证用户名+密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getJwtToken")]
        public async Task<MessageModel<TokenInfoViewModel>> GetJwtToken(string name = "", string pass = "")
        {
            var jwtStr = string.Empty;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                return new MessageModel<TokenInfoViewModel>
                {
                    success = false,
                    msg = "用户名或密码不能为空"
                };
            }

            pass = MD5Helper.MD5Encrypt32(pass);
            var user = await _sysUserInfoServices.Query(a=>a.LoginName==name && a.LoginPwd == pass);
            if (user.Count>0)
            {
                var userRoles = await _sysUserInfoServices.GetUserRoleNameStr(name, pass);
                // 若基于用户授权策略，此处要添加用户；
                // 若基于角色授权策略，此处要添加角色；
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(JwtRegisteredClaimNames.Jti,user.FirstOrDefault().Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_permissionRequirement.Expiration.TotalSeconds).ToString())
                };
                claims.AddRange(userRoles.Split(',').Select(s=>new Claim(ClaimTypes.Role,s)));

                // jwt | ids4
                if (!Permissions.IsUseIds4)
                {
                    var data = await _roleModulePermissionServices.RoleModuleMaps();
                    var list = (from item in data
                                where item.IsDeleted == false
                                orderby item.Id
                                select new PermissionItem
                                {
                                    Url = item.Module?.LinkUrl,
                                    Role = item.Role?.Name.ObjToString()
                                }).ToList();
                    _permissionRequirement.Permissions = list;
                }

                var token = JwtToken.BuildJwtToken(claims.ToArray(), _permissionRequirement);
                return new MessageModel<TokenInfoViewModel> {
                    success = true,
                    msg = "获取成功",
                    response = token
                };
            }
            else
            {
                return new MessageModel<TokenInfoViewModel>
                {
                    success = false,
                    msg = "认证失败"
                };
            }

        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("refreshToken")]
        public async Task<MessageModel<TokenInfoViewModel>> RefreshToken(string token = "")
        {
            var jwtStr = string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                return new MessageModel<TokenInfoViewModel> {
                    success = false,
                    msg = "token无效，请重新登录"
                };
            }

            var tokenModel = JwtHelper.SerializeJWT(token);
            if (tokenModel!=null && tokenModel.Uid>0)
            {
                var user = await _sysUserInfoServices.QueryById(tokenModel.Uid);
                if (user!=null)
                {
                    var userRoles = await _sysUserInfoServices.GetUserRoleNameStr(user.LoginName, user.LoginPwd);
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name,user.LoginName),
                        new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
                        new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_permissionRequirement.Expiration.TotalSeconds).ToString())
                    };
                    claims.AddRange(userRoles.Split(',').Select(a=>new Claim(ClaimTypes.Role,a)));

                    // 用户标识
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaims(claims);

                    var refreshToken = JwtToken.BuildJwtToken(claims.ToArray(), _permissionRequirement);
                    return new MessageModel<TokenInfoViewModel> {
                        success = true,
                        msg = "获取成功",
                        response = refreshToken
                    };
                }
            }

            return new MessageModel<TokenInfoViewModel> {
                success = false,
                msg = "认证失败"
            };
        }
    }
}
