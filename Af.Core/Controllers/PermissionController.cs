using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Extensions.Authorizations.Policys;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionServices _permissionServices;
        private readonly IModuleServices _moduleServices;
        private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IUserRoleServices _userRoleServices;
        private readonly IHttpContextAccessor _httpContext;
        private readonly PermissionRequirement _requirement;

        public PermissionController(IPermissionServices permissionServices, IModuleServices moduleServices, IRoleModulePermissionServices roleModulePermissionServices, IUserRoleServices userRoleServices, IHttpContextAccessor httpContext, PermissionRequirement requirement)
        {
            _permissionServices = permissionServices;
            _moduleServices = moduleServices;
            _roleModulePermissionServices = roleModulePermissionServices;
            _userRoleServices = userRoleServices;
            _httpContext = httpContext;
            _requirement = requirement;
        }

        /// <summary>
        /// 获取导航树数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<NavigationBar>> GetNavigationBar(int uid)
        {
            var data = new MessageModel<NavigationBar>();

            var uidInHttpContext = 0;
            var roleIds = new List<int>();
            // 认证方式切换
            if (Permissions.IsUseIds4)
            {

            }
            else
            {
                uidInHttpContext = (JwtHelper.SerializeJWT(_httpContext.HttpContext.Request.Headers["Authorization"].ObjToString().Replace("Bearer ",""))?.Uid).ObjToInt();
                roleIds = (await _userRoleServices.Query(a => a.IsDeleted == false && a.UserId == uid)).Select(a => a.RoleId).Distinct().ToList();
            }

            if (uid>0 && uid==uidInHttpContext)
            {
                if (roleIds.Any())
                {
                    var pids = (await _roleModulePermissionServices.Query(a => a.IsDeleted == false && roleIds.Contains(a.RoleId))).Select(a => a.PermissionId.ObjToInt()).Distinct();
                    if (pids.Any())
                    {
                        var rolePermissionModules = (await _permissionServices.Query(a => pids.Contains(a.Id))).OrderBy(a=>a.OrderSort);
                        var permissionTrees = (from child in rolePermissionModules
                                               where child.IsDeleted == false
                                               orderby child.Id
                                               select new NavigationBar
                                               {
                                                   id = child.Id,
                                                   name = child.Name,
                                                   pid = child.PId,
                                                   order = child.OrderSort,
                                                   path = child.Code,
                                                   iconCls = child.Icon,
                                                   Func = child.Func,
                                                   IsHide = child.IsHide.ObjToBool(),
                                                   IsButton = child.IsButton.ObjToBool(),
                                                   meta = new NavigationBarMeta
                                                   {
                                                       requireAuth = true,
                                                       title = child.Name,
                                                       NoTabPage = child.IsHide.ObjToBool(),
                                                       keepAlive = child.IskeepAlive.ObjToBool()
                                                   }
                                               }).ToList();

                        NavigationBar rootNavBar = new NavigationBar { 
                            id  =0,
                            pid = 0,
                            order = 0,
                            name = "根节点",
                            path = "",
                            iconCls = "",
                            meta = new NavigationBarMeta ()
                        };

                        permissionTrees = permissionTrees.OrderBy(a=>a.order).ToList();

                        RecursionHelper.LoopNaviBarAppendChildren(permissionTrees, rootNavBar);

                        data.success = true;
                        data.response = rootNavBar;
                        data.msg = "获取成功";
                    }
                }
            }

            return data;
        }
    }
}
