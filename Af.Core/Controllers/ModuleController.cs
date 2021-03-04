using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.HttpContextUser;
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
    public class ModuleController : ControllerBase
    {
        readonly IModuleServices _moduleServices;
        readonly IUser _user;

        public ModuleController(IModuleServices moduleServices, IUser user)
        {
            _moduleServices = moduleServices;
            _user = user;
        }



        /// <summary>
        /// 获取全部api接口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<Module>>> Get(int page =1,int pageSize=50, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";

            Expression<Func<Module, bool>> whereExpression = a => a.IsDeleted == false && (a.Name.Contains(key) || a.LinkUrl.Contains(key));
            var data = await _moduleServices.QueryPage(whereExpression, page, pageSize, " Id Desc");
            return new MessageModel<PageModel<Module>>
            {
                msg = "获取成功",
                response = data,
                success =true
            };
        }

        /// <summary>
        /// 添加一条接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post(Module module)
        {
            var data = new MessageModel<string>();
            module.CreateId = _user.ID;
            module.CreateBy = _user.Name;

            var id = await _moduleServices.Add(module);
            
            if (id>0)
            {
                data.success = true;
                data.msg = "添加成功";
                data.response = id.ObjToString();
            }

            return data;
        }

        /// <summary>
        /// 更新接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put(Module module)
        {
            var data = new MessageModel<string>();
            if (module!=null && module.Id>0)
            {
                data.success = await _moduleServices.Update(module);
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = module.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除一条接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var module = await _moduleServices.QueryById(id);
                module.IsDeleted = true;
                data.success = await _moduleServices.Update(module);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = module.Id.ObjToString();
                }
            }
            return data;
        }
    }
}
