using System;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class ModulePermissionServices : BaseServices<ModulePermission>, IModulePermissionServices
    {
        IBaseRepository<ModulePermission> _dal;

        public ModulePermissionServices(IBaseRepository<ModulePermission> dal)
        {
            _dal = dal;
            base.BaseDal = dal; 
        }
    }
}
