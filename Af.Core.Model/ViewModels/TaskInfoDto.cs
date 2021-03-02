using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.ViewModels
{
    public class TaskInfoDto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string jobId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string jobName { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        public string jobGroup { get; set; }
        /// <summary>
        /// 触发器ID
        /// </summary>
        public string triggerId { get; set; }
        /// <summary>
        /// 触发器名称
        /// </summary>
        public string triggerName { get; set; }
        /// <summary>
        /// 触发器分组
        /// </summary>
        public string triggerGroup { get; set; }
        /// <summary>
        /// 触发器状态
        /// </summary>
        public string triggerStatus { get; set; }
    }
}
