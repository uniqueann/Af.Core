using System;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class PermissionServices : BaseServices<Permission>, IPermissionServices
    {
        IBaseRepository<Permission> _dal;

        public PermissionServices(IBaseRepository<Permission> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
