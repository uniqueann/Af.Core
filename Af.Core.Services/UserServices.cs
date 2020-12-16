using Af.Core.IRepository;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository dal = new UserRepository();

        public List<User> Query(Expression<Func<User, bool>> whereExpression)
        {
            return dal.Query(whereExpression);
        }
    }
}
