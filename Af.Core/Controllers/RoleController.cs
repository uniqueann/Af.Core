using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.HttpContextUser;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/role/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RoleController : ControllerBase
    {
        private readonly IRoleServices _roleService;
        private readonly IUser _user;

        public RoleController(IRoleServices roleService, IUser user)
        {
            _roleService = roleService;
            _user = user;
        }



        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<Role>>> Get(int page=1,string key="")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";
            var data = await _roleService.QueryPage(a=>a.IsDeleted==false && a.Name.Contains(key),page,50,"Id Desc");
            return new MessageModel<PageModel<Role>>
            {
                success = data.Total > 0,
                msg = "获取成功",
                response = data
            };
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody]Role role)
        {

            var data = new MessageModel<string>();

            var roles = await _roleService.Query();
            if (roles.Exists(a=>a.Name.ToLower()==role.Name.ToLower().Trim()))
            {
                data.success = false;
                data.msg = "系统已存在此名称的角色";
                return data;
            }

            role.CreateId = _user.ID;
            role.CreateBy = _user.Name;

            var id = await _roleService.Add(role);
            data.success = id > 0;

            if (data.success)
            {
                data.response = id.ToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put(Role role)
        {
            var data = new MessageModel<string>();
            if (role!= null && role.Id>0)
            {
                role.ModifyBy = _user.Name;
                role.ModifyId = _user.ID;
                data.success = await _roleService.Update(role);
                if (data.success)
                {
                    data.response = role.Id.ToString();
                    data.msg = "更新成功";
                }
            }
            return data;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id>0)
            {
                var role = await _roleService.QueryById(id);
                role.IsDeleted = true;
                data.success = await _roleService.Update(role);
                if (data.success)
                {
                    data.response = role.Id.ToString();
                    data.msg = "删除成功";
                }
            }
            return data;
        }
    }
}
