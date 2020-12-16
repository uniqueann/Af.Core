using Af.Core.IReponsitory;
using Af.Core.Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.Reponsitory
{
    public class BlogArticleReponsitory : IBlogArticleReponsitory
    {
        private DbContext context;
        private SqlSugarClient db;
        private SimpleClient<BlogArticle> entityDb;

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

        public BlogArticleReponsitory()
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
            throw new NotImplementedException();
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
