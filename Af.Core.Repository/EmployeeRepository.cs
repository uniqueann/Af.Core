using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Af.Core.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {

        public EmployeeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<Employee>> GetEmployeeRole()
        {
            var sql = $"SELECT * FROM SystemHelper..Employee a WITH(NOLOCK) JOIN TMSHelper..LCTransportNode b WITH(NOLOCK) ON a.ID = b.CreateUser";
            return await Db.SqlQueryable<Employee>(sql: sql).ToListAsync();
        }
    }
}
