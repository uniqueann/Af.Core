using Af.Core.IRepository.BASE;
using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.IRepository
{
    public interface IRoleModulePermissionRepository:IBaseRepository<RoleModulePermission>
    {
        Task<List<RoleModulePermission>> RoleModuleMaps();
        /// <summary>
        /// 查询角色-菜单-接口关系表全部Map数据
        /// </summary>
        /// <returns></returns>
        Task<List<RoleModulePermission>> GetRMPMaps();
        /// <summary>
        /// 批量更新菜单与接口的关系
        /// </summary>
        /// <param name="permissionId">菜单主键</param>
        /// <param name="moduleId">接口主键</param>
        /// <returns></returns>
        Task UpdateModuleId(int permissionId, int moduleId);
    }
}
