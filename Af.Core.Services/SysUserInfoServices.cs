﻿using Af.Core.Common.Converter;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Services
{
    public class SysUserInfoServices : BaseServices<SysUserInfo>, ISysUserInfoServices
    {
        IBaseRepository<SysUserInfo> _dal;
        IBaseRepository<Role> _roleRepository;
        IBaseRepository<UserRole> _userRoleRepository;

        public SysUserInfoServices(IBaseRepository<SysUserInfo> dal,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<UserRole> userRoleRepository)
        {
            _dal = dal;
            base.BaseDal = dal;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<string> GetUserRoleNameStr(string loginName, string loginPwd)
        {
            var roleName = "";
            var user = (await _dal.Query(a => a.LoginName == loginName && a.LoginPwd == loginPwd)).FirstOrDefault();
            var roleList = await _roleRepository.Query(a=>a.IsDeleted==false);
            if (user!=null)
            {
                var userRoles = await _userRoleRepository.Query(a=>a.UserId==user.Id);
                if (userRoles.Count>0)
                {
                    var arr = userRoles.Select(a=>a.RoleId.ObjToString()).ToList();
                    var roles = roleList.Where(a=>arr.Contains(a.Id.ObjToString()));

                    roleName = string.Join(",",roles.Select(r=>r.Name).ToArray());
                }
            }
            return roleName;
        }

        public async Task<SysUserInfo> SaveUserInfo(string loginName, string loginPwd)
        {
            var sysUserInfo = new SysUserInfo(loginName,loginPwd);
            var model = new SysUserInfo();
            var userList = await _dal.Query(a => a.LoginName == loginPwd && a.LoginPwd == loginPwd);
            if (userList.Count>0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await _dal.Add(sysUserInfo);
                model = await _dal.QueryById(id);
            }

            return model;
        }
    }
}
