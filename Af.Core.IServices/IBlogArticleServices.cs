using Af.Core.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Af.Core.IServices
{
    public interface IBlogArticleServices
    {
        int Sum(int i,int j);
        List<BlogArticle> Query(Expression<Func<BlogArticle,bool>> whereExpression);
    }

}
