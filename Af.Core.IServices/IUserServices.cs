using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.IServices
{
    public interface IUserServices
    {
        List<User> Query(Expression<Func<User, bool>> whereExpression);
    }
}
