using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/role/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetRoles()
        {
            return "admin,normal";
        }
    }
}
