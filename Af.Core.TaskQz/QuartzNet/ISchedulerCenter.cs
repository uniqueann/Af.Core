using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Tasks
{
    public interface ISchedulerCenter
    {
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<MessageModel<string>> StartScheduleAsync();
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<MessageModel<string>> StopScheduleAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<MessageModel<string>> AddScheduleJobAsync(TaskQz taskQz);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<MessageModel<string>> StopScheduleJobAsync(TaskQz taskQz);
        /// <summary>
        /// 检测任务是否存在
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<bool> IsExistScheduleJobAsync(TaskQz taskQz);
        /// <summary>
        /// 暂停指定的计划任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<MessageModel<string>> PauseJob(TaskQz taskQz);
        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<MessageModel<string>> ResumeJob(TaskQz taskQz);

        /// <summary>
        /// 获取任务触发器状态
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<List<TaskInfoDto>> GetTaskStaus(TaskQz taskQz);
        /// <summary>
        /// 获取触发器标识
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetTriggerState(string key);
    }
}
