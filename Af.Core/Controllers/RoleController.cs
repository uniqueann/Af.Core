using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.IServices;
using Af.Core.Model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/role/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly IEmployeeServices _empServices;

        public RoleController(IEmployeeServices empServices)
        {
            _empServices = empServices;
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetRoles()
        {
            return "admin,normal";
        }

        /// <summary>
        /// 根据主键查询员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<EmployeeViewModel> GetEmployee(int id)
        {
            return await _empServices.GetEmployee(id);
        }
    }
}
