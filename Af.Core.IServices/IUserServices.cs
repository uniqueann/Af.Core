using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using System.Threading.Tasks;

namespace Af.Core.IServices
{
    public interface IUserServices : IBaseServices<User>
    {
        Task<UserViewModel> GetUser(int id);
        Task<PageModel<UserViewModel>> GetUserList(int pageIndex, int pageSize, string userName);
        
    }
}
