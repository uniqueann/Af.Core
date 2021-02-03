using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using System.Threading.Tasks;

namespace Af.Core.IServices
{
    public interface IEmployeeServices : IBaseServices<Employee>
    {
        Task<EmployeeViewModel> GetEmployee(int id);
        Task<PageModel<EmployeeViewModel>> GetEmployeeList(int pageIndex, int pageSize, string employeeName);
    }
}
