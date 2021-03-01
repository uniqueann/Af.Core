using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Af.Core.IServices
{
    public interface IRoleModulePermissionServices : IBaseServices<RoleModulePermission>
    {
        Task<List<RoleModulePermission>> GetRoleModule();
        Task<List<RoleModulePermission>> RoleModuleMaps();
        Task<List<RoleModulePermission>> GetRMPMaps();
        Task UpdateModuleId(int permissionId, int moduleId);
    }
}
