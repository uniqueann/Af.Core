using Af.Core.IReponsitory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Reponsitory
{
    public class BlogArticleReponsitory : IBlogArticleReponsitory
    {
        public int Sum(int i, int j)
        {
            return i + j;
        }
    }
}
