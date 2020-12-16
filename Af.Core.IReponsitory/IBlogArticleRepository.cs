using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Af.Core.IRepository
{
    public interface IBlogArticleRepository
    {
        int Sum(int i, int j);

        int Add(BlogArticle model);

        bool Delete(BlogArticle model);

        bool Update(BlogArticle model);

        List<BlogArticle> Query(Expression<Func<BlogArticle, bool>> whereExpression);
    }
}
