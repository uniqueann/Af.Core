using System;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class UserRoleServices:BaseServices<UserRole>,IUserRoleServices
    {
        IBaseRepository<UserRole> _dal;

        public UserRoleServices(IBaseRepository<UserRole> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
