using Af.Core.Common.DB;
using Af.Core.IRepository;
using Af.Core.Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.Repository
{
    public class BlogArticleRepository : IBlogArticleRepository
    {
        private DbContext context;
        private SqlSugarClient db;
        private readonly SimpleClient<BlogArticle> entityDb;

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

        public BlogArticleRepository()
        {
            DbContext.Init(BaseDBConfig.ConnectionString);
            context = DbContext.GetDbContext();
            db = context.Db;
            entityDb = context.GetEntityDB<BlogArticle>(Db);
        }

        public int Add(BlogArticle model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(BlogArticle model)
        {
            throw new NotImplementedException();
        }

        public List<BlogArticle> Query(Expression<Func<BlogArticle, bool>> whereExpression)
        {
            return db.Queryable<BlogArticle>().ToList();
        }

        public int Sum(int i, int j)
        {
            return i + j;
        }

        public bool Update(BlogArticle model)
        {
            throw new NotImplementedException();
        }
    }
}
