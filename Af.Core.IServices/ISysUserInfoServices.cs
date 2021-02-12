using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using System.Threading.Tasks;

namespace Af.Core.IServices
{
    public interface ISysUserInfoServices:IBaseServices<SysUserInfo>
    {
        Task<SysUserInfo> SaveUserInfo(string loginName,string loginPwd);
        Task<string> GetUserRoleNameStr(string loginName,string loginPwd);
    }
}
