using Af.Core.Common;
using Af.Core.IRepository;
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
    public class RoleModulePermissionServices : BaseServices<RoleModulePermission>, IRoleModulePermissionServices
    {
        private readonly IRoleModulePermissionRepository _dal;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<Module> _moduleRespository;

        public RoleModulePermissionServices(IRoleModulePermissionRepository dal, IBaseRepository<Role> roleRepository,
                                            IBaseRepository<Module> moduleRespository)
        {
            _dal = dal;
            _roleRepository = roleRepository;
            _moduleRespository = moduleRespository;
            base.BaseDal = dal;
        }

        [Caching(AbsoluteExpiration = 10)]
        public async Task<List<RoleModulePermission>> GetRMPMaps()
        {
            return await _dal.GetRMPMaps();
        }

        public async Task<List<RoleModulePermission>> GetRoleModule()
        {
            var roleModulePermissions = await base.Query(a => a.IsDeleted == false);
            var roles = await _roleRepository.Query(a => a.IsDeleted == false);
            var modules = await _moduleRespository.Query(a => a.IsDeleted == false);

            if (roleModulePermissions.Count > 0)
            {
                foreach (var item in roleModulePermissions)
                {
                    item.Role = roles.FirstOrDefault(r => r.Id == item.RoleId);
                    item.Module = modules.FirstOrDefault(m => m.Id == item.ModuleId);
                }
            }

            return roleModulePermissions;
        }

        public async Task<List<RoleModulePermission>> RoleModuleMaps()
        {
            return await _dal.RoleModuleMaps();
        }
    }
}
