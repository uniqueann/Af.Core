﻿using SqlSugar;
using System;

namespace Af.Core.Model.Models
{
    [SugarTable("AF_OperateLog", "CrawlerHelper")]
    public class OperateLog
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// 区域名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Area { get; set; }
        /// <summary>
        /// 区域控制器名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Controller { get; set; }
        /// <summary>
        /// Action名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Action { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string IPAddress { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Description { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? LogTime { get; set; }
        /// <summary>
        /// 登录名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string LoginName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        //[SugarColumn(IsIgnore = true)]
        //public virtual sysUserInfo User { get; set; }
    }
}
