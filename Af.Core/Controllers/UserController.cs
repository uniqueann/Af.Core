using Af.Core.Common;
using Af.Core.Common.Helper;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Af.Core.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// 根据主键查询一个用户的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        public async Task<User> Get(int id)
        {
            return await _userServices.QueryByID(id);
        }

        /// <summary>
        /// 分页查询用户列表 可根据名称模糊查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 10)]
        [HttpGet("page_index/{pageIndex}/page_size/{pageSize}")]
        public async Task<PageModel<User>> Get(int pageIndex, int pageSize, string userName)
        {
            return await _userServices.QueryPage(a => a.IsEnable && a.UserName.Contains(userName.ObjToString()), pageIndex, pageSize, "CreateTime desc");
        }

        /// <summary>
        /// 根据主键修改用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> Put(User user)
        {
            return await _userServices.Update(user);
        }
    }
}
