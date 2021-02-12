using System;
using System.Collections.Generic;
using System.Text;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class RoleServices:BaseServices<Role>,IRoleServices
    {
        IBaseRepository<Role> _dal;

        public RoleServices(IBaseRepository<Role> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
