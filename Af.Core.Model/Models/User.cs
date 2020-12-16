using SqlSugar;
using System;

namespace Af.Core.Model.Models
{
    [SugarTable("Users")]
    public class User
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int UserId { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string UserName { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string Password { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string Email { get; set; }
        [SugarColumn(ColumnDataType = "bit", IsNullable = false)]
        public bool IsEnable { get; set; }
        public DateTime Expiration { get; set; }
        public int MonitorItemLimit { get; set; } = 0;
        public int MonitorStoreLimit { get; set; } = 0;
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string LastUpdUser { get; set; }
        public DateTime LastUpdTime { get; set; }
    }
}
