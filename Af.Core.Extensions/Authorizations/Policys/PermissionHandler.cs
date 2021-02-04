using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Af.Core.Extensions.Authorizations.Policys
{
    /// <summary>
    /// 权限授权处理器
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IHttpContextAccessor _accessor;

        public PermissionHandler(IAuthenticationSchemeProvider schemes, IRoleModulePermissionServices roleModulePermissionServices, IHttpContextAccessor accessor)
        {
            Schemes = schemes;
            _roleModulePermissionServices = roleModulePermissionServices;
            _accessor = accessor;
        }
        // 重写异步处理程序
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = _accessor.HttpContext;

            //获取系统中所有角色和菜单关系的集合
            if (!requirement.Permissions.Any())
            {
                var list = new List<PermissionItem>();
                //jwt

            }

            if (httpContext != null)
            {
                var reqUrl = httpContext.Request.Path.Value.ToLower();

                httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = httpContext.Request.Path,
                    OriginalPathBase = httpContext.Request.PathBase
                });

                // 判断当前是否需要远程验证
                var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        context.Fail();
                        return;
                    }
                }

                // 判断请求是否拥有凭证 即是否已经登录
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                    //是否开启测试环境
                    var isTestCurrent = Appsettings.app(new string[] { "AppSettings", "UseLoadTest" }).ObjToBool();
                    if (isTestCurrent || result?.Principal != null)
                    {
                        if (!isTestCurrent)
                        {
                            httpContext.User = result.Principal;
                        }

                        //获取当前用户的角色消息
                        var currentUserRoles = new List<string>();
                        //jwt
                        currentUserRoles = (from item in httpContext.User.Claims
                                            where item.Type == requirement.ClaimType
                                            select item.Value).ToList();

                        var isMatchRole = false;
                        var permissionRoles = requirement.Permissions.Where(a => currentUserRoles.Contains(a.Role));
                        foreach (var item in permissionRoles)
                        {
                            try
                            {
                                if (Regex.Match(reqUrl, item.Url?.ObjToString().ToLower())?.Value == reqUrl)
                                {
                                    isMatchRole = true;
                                    break;
                                }
                            }
                            catch (Exception) { }
                        }

                        // 验证权限
                        if (currentUserRoles.Count <= 0 || !isMatchRole)
                        {
                            context.Fail();
                            return;
                        }

                        var isExp = false;
                        //jwt
                        isExp = httpContext.User.Claims.SingleOrDefault(a =>
                        {
                            return a.Type == ClaimTypes.Expiration;
                        })?.Value != null && DateHelper.StampToDateTime(httpContext.User.Claims.SingleOrDefault(a =>
                        {
                            return a.Type == ClaimTypes.Expiration;
                        })?.Value) >= DateTime.Now;

                        if (isExp)
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                        return;
                    }
                }

                // 判断没有登录时 是否访问登录的url && post请求 && form表单提交   否则为失败
                if (!reqUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) && (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType))
                {
                    context.Fail();
                    return;
                }
            }
        }
    }
}
