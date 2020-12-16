using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.IReponsitory
{
    public interface IBlogArticleReponsitory
    {
        int Sum(int i,int j);

        int Add(BlogArticle model);

        bool Delete(BlogArticle model);

        bool Update(BlogArticle model);

        List<BlogArticle> Query(Expression<Func<BlogArticle, bool>> whereExpression);
    }
}
