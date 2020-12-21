using Af.Core.IRepository;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using Af.Core.Repository;
using Af.Core.Services.BASE;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.Services
{
    public class UserServices : BaseServices<User>, IUserServices
    {
        public UserServices(IBaseRepository<User> balDal) : base(balDal)
        {
        }
    }
}
