using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.Models
{
    public class PageModel<T>
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
        public List<T> PageData { get; set; }
    }
}
