using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using System.Threading.Tasks;

namespace Af.Core.IServices
{
    public interface IUserServices : IBaseServices<User>
    {
        Task<UserViewModel> GetUser(int id);
    }
}
