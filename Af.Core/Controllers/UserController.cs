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

        [HttpGet("page_index/{pageIndex}/page_size/{pageSize}")]
        public async Task<PageModel<User>> Get(int pageIndex,int pageSize)
        {
            return await _userServices.QueryPage(a => a.IsEnable, 1, 20);
        }

        //[HttpGet]
        //public async Task<PageModel<User>> GetList()
        //{
        //    return await _userServices.QueryPage(a=>a.IsEnable,1,20);
        //}

        [HttpPut]
        public async Task<bool> Put(User user)
        {
            return await _userServices.Update(user);
        }
    }
}
