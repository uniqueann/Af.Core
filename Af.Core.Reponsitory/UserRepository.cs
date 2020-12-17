using Af.Core.Common.DB;
using Af.Core.IRepository;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        
    }
}
