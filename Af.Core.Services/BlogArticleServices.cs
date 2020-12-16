using Af.Core.IRepository;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.Services
{
    public class BlogArticleServices:IBlogArticleServices
    {
        IBlogArticleRepository dal = new BlogArticleRepository();

        public List<BlogArticle> Query(Expression<Func<BlogArticle, bool>> whereExpression)
        {
            return dal.Query(whereExpression);
        }

        public int Sum(int i, int j)
        {
            return dal.Sum(i,j);
        }
    }
}
