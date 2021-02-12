using System;
using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;

namespace Af.Core.Repository
{
    public class RoleRepository:BaseRepository<Role>,IRoleRepository
    {
        public RoleRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {
        }
    }
}
