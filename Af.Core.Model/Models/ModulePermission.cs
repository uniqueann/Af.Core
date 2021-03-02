using SqlSugar;
using System;

namespace Af.Core.Model.Models
{
    [SugarTable("AF_ModulePermission", "CrawlerHelper")]
    public class ModulePermission
    { 
        public ModulePermission()
        {
            IsDeleted = false;
            CreateTime = DateTime.Now;
            ModifyTime = DateTime.Now;
        }

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public int ModuleId { get; set; }

        [SugarColumn(IsNullable = false)]
        public int PermissionId { get; set; }

        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsDeleted { get; set; }

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
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? ModifyId { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = true)]
        public string ModifyBy { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }
    }
}
