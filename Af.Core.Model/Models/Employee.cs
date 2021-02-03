using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.Models
{
    [SugarTable("Employee","SystemHelper")]
    public class Employee
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string LoginName { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string EmployeeName { get; set; }
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = false)]
        public string Password { get; set; }

        public int OrgId { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public DateTime UpdTime { get; set; }
    }

}
