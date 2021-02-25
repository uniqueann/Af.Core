using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.HttpContextUser;
using Af.Core.Extensions.Authorizations.Policys;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionServices _permissionServices;
        private readonly IModuleServices _moduleServices;
        private readonly IRoleModulePermissionServices _roleModulePermissionServices;
        private readonly IUserRoleServices _userRoleServices;
        private readonly IHttpContextAccessor _httpContext;
        private readonly PermissionRequirement _requirement;
        private readonly IUser _user;

        public PermissionController(IPermissionServices permissionServices, IModuleServices moduleServices, IRoleModulePermissionServices roleModulePermissionServices, IUserRoleServices userRoleServices, IHttpContextAccessor httpContext, PermissionRequirement requirement, IUser user)
        {
            _permissionServices = permissionServices;
            _moduleServices = moduleServices;
            _roleModulePermissionServices = roleModulePermissionServices;
            _userRoleServices = userRoleServices;
            _httpContext = httpContext;
            _requirement = requirement;
            _user = user;
        }



        /// <summary>
        /// 获取导航树数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
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

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<Permission>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";

            var permissions = new PageModel<Permission>();

            permissions = await _permissionServices.QueryPage(a => a.IsDeleted == false && (a.Name.Contains(key) || a.Code.Contains(key)), page, 50, "Id Desc");

            var apis = await _moduleServices.Query(a => a.IsDeleted == false);
            var permissionView = permissions.PageData;

            var permissionAll = await _permissionServices.Query(a => a.IsDeleted == false);
            foreach (var item in permissionView)
            {
                var pidArr = new List<int> {
                    item.PId
                };
                if (item.PId > 0)
                {
                    pidArr.Add(0);
                }
                var parent = permissionAll.FirstOrDefault(a => a.Id == item.PId);

                while (parent != null)
                {
                    pidArr.Add(parent.Id);
                    parent = permissionAll.FirstOrDefault(a => a.Id == parent.PId);
                }

                item.PidArr = pidArr.OrderBy(a => a).Distinct().ToList();
                foreach (var pid in item.PidArr)
                {
                    var per = permissionAll.FirstOrDefault(a => a.Id == pid);
                    item.PnameArr.Add((per != null ? per.Name : "根节点") + "/");
                }
                item.ModuleName = apis.FirstOrDefault(a => a.Id == item.ModuleId)?.LinkUrl;
            }

            permissions.PageData = permissionView;

            return new MessageModel<PageModel<Permission>>
            {
                msg = "获取成功",
                response = permissions,
                success = permissions.Total > 0
            };
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="needbtn"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<PermissionTree>> GetPermissionTree(int pid = 0, bool needbtn = false)
        {
            var data = new MessageModel<PermissionTree>();
            var permissions = await _permissionServices.Query(a => a.IsDeleted == false);
            var permissionTrees = (from child in permissions
                                   where child.IsDeleted == false
                                   orderby child.Id
                                   select new PermissionTree
                                   {
                                       value = child.Id,
                                       label = child.Name,
                                       Pid = child.PId,
                                       isbtn = child.IsButton,
                                       order = child.OrderSort
                                   }).ToList();

            var rootNode = new PermissionTree
            {
                value = 0,
                Pid = 0,
                label = "根节点"
            };
            permissionTrees = permissionTrees.OrderBy(a => a.order).ToList();

            RecursionHelper.LoopToAppendChildren(permissionTrees, rootNode, pid, needbtn);

            data.success = true;
            data.response = rootNode;
            data.msg = "获取成功";

            return data;
        }

        /// <summary>
        /// 获取树形table
        /// </summary>
        /// <param name="f"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<Permission>>> GetTreeTable(int f=0,string key = "")
        {
            var permissions = new List<Permission>();
            var apiList = await _moduleServices.Query(a=>a.IsDeleted==false);
            var permissionList = await _permissionServices.Query(a=>a.IsDeleted==false);

            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";

            if (key!="")
            {
                permissions = permissionList.Where(a=>a.Name.Contains(key)).OrderBy(a=>a.OrderSort).ToList();
            }
            else
            {
                permissions = permissionList.Where(a => a.PId == f).OrderBy(a=>a.OrderSort).ToList();
            }

            foreach (var item in permissions)
            {
                var pidArr = new List<int> { };
                var parent = permissionList.FirstOrDefault(a=>a.Id==item.PId);

                while (parent!=null)
                {
                    pidArr.Add(parent.Id);
                    parent = permissionList.FirstOrDefault(a=>a.Id==parent.PId);
                }
                pidArr.Reverse();
                pidArr.Insert(0,0);
                item.PidArr = pidArr;

                item.ModuleName = apiList.FirstOrDefault(a => a.Id == item.ModuleId)?.LinkUrl;
                item.HasChildren = permissionList.Where(a => a.PId == item.Id).Any();
            }

            return new MessageModel<List<Permission>> {
                msg = "获取成功",
                success = true,
                response = permissions
            };
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post(Permission permission)
        {
            var data = new MessageModel<string>();
            permission.CreateId = _user.ID;
            permission.CreateBy = _user.Name;

            var id = await _permissionServices.Add(permission);
            if (id>0)
            {
                data.success = true;
                data.msg = "添加成功";
                data.response = id.ObjToString();
            }

            return data;
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put(Permission permission)
        {
            var data = new MessageModel<string>();
            if (permission!= null && permission.Id>0)
            {
                permission.ModifyId = _user.ID;
                permission.ModifyBy = _user.Name;
                data.success = await _permissionServices.Update(permission);
                if (data.success)
                {
                    data.response = permission.Id.ToString();
                    data.msg = "更新成功";
                }
            }

            return data;
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id>0)
            {
                var permission = await _permissionServices.QueryById(id);
                permission.IsDeleted = true;
                data.success = await _permissionServices.Update(permission);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = permission.Id.ToString();
                }
            }
            return data;
        }
    }
}
