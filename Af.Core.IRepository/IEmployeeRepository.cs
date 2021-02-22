using Af.Core.IRepository.BASE;
using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.IRepository
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<List<Employee>> GetEmployeeRole();
    }
}
