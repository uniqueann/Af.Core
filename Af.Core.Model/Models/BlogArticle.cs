using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.Models
{
    public class BlogArticle
    {
        [SugarColumn(IsNullable =false,IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar",Length =50, IsNullable = false)]
        public string Title { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = false)]
        public string Subject { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = false)]
        public string Content { get; set; }

    }
}
