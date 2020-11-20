using Af.Core.IReponsitory;
using Af.Core.IServices;
using Af.Core.Reponsitory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Services
{
    public class BlogArticleServices:IBlogArticleServices
    {
        IBlogArticleReponsitory dal = new BlogArticleReponsitory();

        public int Sum(int i, int j)
        {
            return dal.Sum(i,j);
        }
    }
}
