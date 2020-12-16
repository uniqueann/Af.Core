using Af.Core.Common.DB;
using Af.Core.IRepository;
using Af.Core.Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.Repository
{
    public class UserRepository : IUserRepository
    {
        private DbContext context;
        private SqlSugarClient db;
        private readonly SimpleClient<User> entityDb;

        internal SqlSugarClient Db
        {
            get { return db; }
            private set { db = value; }
        }

        public DbContext Context
        {
            get { return context; }
            set { context = value; }
        }

        public UserRepository()
        {
            DbContext.Init(BaseDBConfig.ConnectionString);
            context = DbContext.GetDbContext();
            db = context.Db;
            entityDb = context.GetEntityDB<User>(Db);
        }

        public int Add(User model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(User model)
        {
            throw new NotImplementedException();
        }

        public List<User> Query(Expression<Func<User, bool>> whereExpression)
        {
            return db.Queryable<User>().Where(whereExpression).ToList();
        }

        public bool Update(User model)
        {
            throw new NotImplementedException();
        }
    }
}
