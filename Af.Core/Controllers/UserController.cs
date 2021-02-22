using Af.Core.Common;
using Af.Core.Common.Helper;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Af.Core.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ISysUserInfoServices _sysUserInfoService;

        public UserController(ISysUserInfoServices sysUserInfoService)
        {
            
            _sysUserInfoService = sysUserInfoService;
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<SysUserInfo>> GetInfoByToken(string token)
        {
            var data = new MessageModel<SysUserInfo>();
            if (!string.IsNullOrEmpty(token))
            {
                var tokenModel = JwtHelper.SerializeJWT(token);
                if (tokenModel!=null && tokenModel.Uid>0)
                {
                    var userInfo = await _sysUserInfoService.QueryById(tokenModel.Uid);
                    if (userInfo!=null)
                    {
                        data.response = userInfo;
                        data.success = true;
                        data.msg = "获取成功";
                    }
                }
            }
            return data;
        }

        ///// <summary>
        ///// 分页查询用户列表 可根据名称模糊查询
        ///// </summary>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="userName"></param>
        ///// <returns></returns>
        //[Caching(AbsoluteExpiration = 10)]
        //[HttpGet("page_index/{pageIndex}/page_size/{pageSize}")]
        //public async Task<PageModel<UserViewModel>> Get(int pageIndex, int pageSize, string userName)
        //{
        //    return await _userServices.GetUserList(pageIndex, pageSize, userName);
        //}

    }
}
