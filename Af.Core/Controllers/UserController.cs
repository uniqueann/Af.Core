using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.HttpContextUser;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Af.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysUserInfoServices _sysUserInfoService;
        private readonly IRoleServices _roleServices;
        private readonly IUserRoleServices _userRoleServices;
        private readonly IUser _user;
        private readonly ILogger<UserController> _logger;

        public UserController(IUnitOfWork unitOfWork, ISysUserInfoServices sysUserInfoService, IRoleServices roleServices, IUserRoleServices userRoleServices, IUser user, ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _sysUserInfoService = sysUserInfoService;
            _roleServices = roleServices;
            _userRoleServices = userRoleServices;
            _user = user;
            _logger = logger;
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
                    var roles = await _roleServices.Query(a=>a.IsDeleted==false);
                    var userRoles = await _userRoleServices.Query(a=>a.IsDeleted==false && a.UserId==userInfo.Id);
                    userInfo.RoleIds = userRoles.Select(a => a.RoleId).ToList();
                    userInfo.RoleNames = roles.Where(a => userInfo.RoleIds.Contains(a.Id)).Select(a => a.Name).ToList();
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

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<PageModel<SysUserInfo>>> Get(int page=1,string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) key = "";
            var data = await _sysUserInfoService.QueryPage(a =>a.Status>=0 && (a.LoginName.Contains(key) || a.RealName.Contains(key)), page, 50, "Id Desc");
            var roles = await _roleServices.Query(a => a.IsDeleted == false);
            var userRoles = await _userRoleServices.Query(a=>a.IsDeleted==false);

            foreach (var item in data.PageData)
            {
                var currentUserRoles = userRoles.Where(a => a.UserId == item.Id).Select(a => a.RoleId).ToList();

                item.RoleIds = currentUserRoles;
                item.RoleNames = roles.Where(a => currentUserRoles.Contains(a.Id)).Select(a => a.Name).ToList();
            }

            return new MessageModel<PageModel<SysUserInfo>> { 
                success = data.Total>0,
                msg="获取成功",
                response = data
            };
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> Post(SysUserInfo user)
        {
            var data = new MessageModel<string>();
            try
            {
                _unitOfWork.BeginTran();

                user.CreateId = _user.ID;
                user.CreateBy = _user.Name;
                user.LoginPwd = MD5Helper.MD5Encrypt32(user.LoginPwd);

                var id = await _sysUserInfoService.Add(user);

                if (user.RoleIds.Count>0)
                {
                    var userRoles = new List<UserRole>();
                    user.RoleIds.ForEach(rid=> {
                        userRoles.Add(new UserRole { 
                            UserId = id,
                            RoleId = rid,
                            CreateBy = _user.Name,
                            CreateId = _user.ID
                        });
                    });
                    await _userRoleServices.Add(userRoles);
                }                

                _unitOfWork.CommitTran();

                if (id > 0)
                {
                    data.success = true;
                    data.msg = "添加成功";
                    data.response = id.ObjToString();
                }
            }
            catch (Exception e)
            {
                _unitOfWork.RollbackTran();
                _logger.LogError(e,e.Message);
            }
            
            return data;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> Put(SysUserInfo user)
        {
            var data = new MessageModel<string>();
            try
            {
                _unitOfWork.BeginTran();

                user.ModifyBy = _user.Name;
                user.ModifyId = _user.ID;

                if (user.RoleIds.Count>0)
                {
                    var userRoles = (await _userRoleServices.Query(a => a.UserId == user.Id)).Select(a => a.Id.ToString()).ToArray();
                    if (userRoles.Count() > 0)
                    {
                        await _userRoleServices.DeleteByIds(userRoles);
                    }
                    var userRolesAdd = new List<UserRole>();
                    user.RoleIds.ForEach(rid=> {
                        userRolesAdd.Add(new UserRole { 
                            UserId = user.Id,
                            RoleId = rid,
                            CreateBy = _user.Name,
                            CreateId = _user.ID
                        });
                    });
                    await _userRoleServices.Add(userRolesAdd);
                }

                data.success = await _sysUserInfoService.Update(user);
                _unitOfWork.CommitTran();
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = user.Id.ToString();
                }
            }
            catch (Exception e)
            {
                _unitOfWork.RollbackTran();
                _logger.LogError(e, e.Message);
            }
            return data;
        }
    }
}
