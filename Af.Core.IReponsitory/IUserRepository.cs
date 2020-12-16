using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.IRepository
{
    public interface IUserRepository
    {
        int Add(User model);

        bool Delete(User model);

        bool Update(User model);

        List<User> Query(Expression<Func<User, bool>> whereExpression);
    }
}
