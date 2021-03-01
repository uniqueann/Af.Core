using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Repository
{
    public class RoleModulePermissionRepository : BaseRepository<RoleModulePermission>, IRoleModulePermissionRepository
    {
        public RoleModulePermissionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<RoleModulePermission>> GetRMPMaps()
        {
            return await Db.Queryable<RoleModulePermission>()
                           .Mapper(rmp => rmp.Module, rmp => rmp.ModuleId)
                           .Mapper(rmp => rmp.Permission, rmp => rmp.PermissionId)
                           .Mapper(rmp => rmp.Role, rmp => rmp.RoleId)
                           .Where(d => d.IsDeleted == false)
                           .ToListAsync();
        }

        public async Task<List<RoleModulePermission>> RoleModuleMaps()
        {
            return await QueryMuch<RoleModulePermission, Module, Role, RoleModulePermission>(
                (rmp, m, r) => new object[] {
                    JoinType.Left, rmp.ModuleId == m.Id,
                    JoinType.Left, rmp.RoleId == r.Id
                },
                (rmp, m, r) => new RoleModulePermission()
                {
                    Role = r,
                    Module = m,
                    IsDeleted = rmp.IsDeleted
                },
                (rmp, m, r) => rmp.IsDeleted == false && m.IsDeleted == false && r.IsDeleted == false
                );
        }

        public async Task UpdateModuleId(int permissionId, int moduleId)
        {
            await Db.Updateable<RoleModulePermission>(a => a.ModuleId == moduleId).Where(a=> a.PermissionId == permissionId)
                .ExecuteCommandAsync();
        }
    }
}
