using System;
using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;

namespace Af.Core.Repository
{
    public class SysUserInfoRepository:BaseRepository<SysUserInfo>,ISysUserInfoRepository
    {
        public SysUserInfoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
