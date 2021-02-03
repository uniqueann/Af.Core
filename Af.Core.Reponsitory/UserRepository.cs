using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;

namespace Af.Core.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
