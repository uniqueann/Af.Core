using SqlSugar;
using System;
using System.Collections.Generic;

namespace Af.Core.Model.Models
{
    [SugarTable("Users","CrawlerHelper")]
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

    [SugarTable("AF_SysUserInfo","CrawlerHelper")]
    public class SysUserInfo
    {
        public SysUserInfo() { }

        public SysUserInfo(string loginName, string loginPwd)
        {
            LoginName = loginName;
            LoginPwd = loginPwd;
            Status = 0;
            Remark = string.Empty;
            CreateTime = DateTime.Now;
            ModifyTime = DateTime.Now;
        }

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string LoginName { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string LoginPwd { get; set; }

        public int Status { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 500, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 创建ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? CreateId { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = true)]
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? ModifyId { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ModifyBy { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifyTime { get; set; } = DateTime.Now;

        [SugarColumn(IsIgnore = true)]
        public List<string> RoleNames { get; set; } 
    }
}
