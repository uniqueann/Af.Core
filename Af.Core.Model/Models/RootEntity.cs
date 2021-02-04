using SqlSugar;

namespace Af.Core.Model.Models
{
    public class RootEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }
    }
}
