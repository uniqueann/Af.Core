using Af.Core.Common.Converter;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;

namespace Af.Core.Tasks
{
    public class SchedulerCenterServer : ISchedulerCenter
    {
        private Task<IScheduler> _scheduler;
        private readonly IJobFactory _jobFactory;

        public SchedulerCenterServer(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
            _scheduler = GetSchedulerAsync();
        }

        private Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
            {
                return this._scheduler;
            }
            else
            {
                // 从Factory中获取Scheduler实例
                NameValueCollection collection = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" },
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(collection);
                return _scheduler = factory.GetScheduler();
            }
        }
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> StartScheduleAsync()
        {
            var result = new MessageModel<string>();
            try
            {
                this._scheduler.Result.JobFactory = this._jobFactory;
                if (!this._scheduler.Result.IsStarted)
                {
                    //等待任务运行完成
                    await this._scheduler.Result.Start();
                    await Console.Out.WriteLineAsync("任务调度开启！");
                    result.success = true;
                    result.msg = $"任务调度开启成功";
                    return result;
                }
                else
                {
                    result.success = false;
                    result.msg = $"任务调度已经开启";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<MessageModel<string>> StopScheduleAsync()
        {
            var result = new MessageModel<string>();
            try
            {
                if (!this._scheduler.Result.IsShutdown)
                {
                    //等待任务运行完成
                    await this._scheduler.Result.Shutdown();
                    await Console.Out.WriteLineAsync("任务调度停止！");
                    result.success = true;
                    result.msg = $"任务调度停止成功";
                    return result;
                }
                else
                {
                    result.success = false;
                    result.msg = $"任务调度已经停止";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加一个计划任务
        /// </summary>
        /// <param name="taskQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> AddScheduleJobAsync(TaskQz taskQz)
        {
            var result = new MessageModel<string>();

            if (taskQz != null)
            {
                try
                {
                    JobKey jobKey = new JobKey(taskQz.Id.ToString(), taskQz.JobGroup);
                    if (await _scheduler.Result.CheckExists(jobKey))
                    {
                        result.success = false;
                        result.msg = $"该任务计划已经在执行:【{taskQz.Name}】,请勿重复启动！";
                        return result;
                    }
                    #region 设置开始时间和结束时间

                    if (taskQz.BeginTime == null)
                    {
                        taskQz.BeginTime = DateTime.Now;
                    }
                    DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(taskQz.BeginTime, 1);//设置开始时间
                    if (taskQz.EndTime == null)
                    {
                        taskQz.EndTime = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(taskQz.EndTime, 1);//设置暂停时间

                    #endregion

                    #region 通过反射获取程序集类型和类   

                    Assembly assembly = Assembly.Load(new AssemblyName(taskQz.AssemblyName));
                    Type jobType = assembly.GetType(taskQz.AssemblyName + "." + taskQz.ClassName);

                    #endregion
                    //判断任务调度是否开启
                    if (!_scheduler.Result.IsStarted)
                    {
                        await StartScheduleAsync();
                    }

                    //传入反射出来的执行程序集
                    IJobDetail job = new JobDetailImpl(taskQz.Id.ToString(), taskQz.JobGroup, jobType);
                    job.JobDataMap.Add("JobParam", taskQz.JobParams);
                    ITrigger trigger;

                    #region 泛型传递
                    //IJobDetail job = JobBuilder.Create<T>()
                    //    .WithIdentity(sysSchedule.Name, sysSchedule.JobGroup)
                    //    .Build();
                    #endregion

                    if (taskQz.Cron != null && CronExpression.IsValidExpression(taskQz.Cron) && taskQz.TriggerType > 0)
                    {
                        trigger = CreateCronTrigger(taskQz);

                        ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    }
                    else
                    {
                        trigger = CreateSimpleTrigger(taskQz);
                    }

                    // 告诉Quartz使用我们的触发器来安排作业
                    await _scheduler.Result.ScheduleJob(job, trigger);
                    result.success = true;
                    result.msg = $"【{taskQz.Name}】成功";
                    return result;
                }
                catch (Exception ex)
                {
                    result.success = false;
                    result.msg = $"任务计划异常:【{ex.Message}】";
                    return result;
                }
            }
            else
            {
                result.success = false;
                result.msg = $"任务计划不存在:【{taskQz?.Name}】";
                return result;
            }
        }

        /// <summary>
        /// 暂停指定的计划任务
        /// </summary>
        /// <param name="taskQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> StopScheduleJobAsync(TaskQz taskQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(taskQz.Id.ToString(), taskQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.success = false;
                    result.msg = $"未找到要暂停的任务:【{taskQz.Name}】";
                    return result;
                }
                else
                {
                    await this._scheduler.Result.DeleteJob(jobKey);
                    result.success = true;
                    result.msg = $"【{taskQz.Name}】成功";
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 任务是否存在?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsExistScheduleJobAsync(TaskQz sysSchedule)
        {
            JobKey jobKey = new JobKey(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 暂停指定的计划任务
        /// </summary>
        /// <param name="taskQz"></param>
        /// <returns></returns>
        public async Task<MessageModel<string>> PauseJob(TaskQz taskQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(taskQz.Id.ToString(), taskQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.success = false;
                    result.msg = $"未找到要暂停的任务:【{taskQz.Name}】";
                    return result;
                }
                await this._scheduler.Result.PauseJob(jobKey);
                result.success = true;
                result.msg = $"【{taskQz.Name}】成功";
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MessageModel<string>> ResumeJob(TaskQz taskQz)
        {
            var result = new MessageModel<string>();
            try
            {
                JobKey jobKey = new JobKey(taskQz.Id.ToString(), taskQz.JobGroup);
                if (!await _scheduler.Result.CheckExists(jobKey))
                {
                    result.success = false;
                    result.msg = $"未找到要恢复的任务:【{taskQz.Name}】";
                    return result;
                }
                await this._scheduler.Result.ResumeJob(jobKey);
                result.success = true;
                result.msg = $"【{taskQz.Name}】成功";
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }



        #region 状态相关方法
        public async Task<List<TaskInfoDto>> GetTaskStaus(TaskQz taskQz)
        {
            var ls = new List<TaskInfoDto>();
            var noTask = new List<TaskInfoDto>{ new TaskInfoDto {
                jobId = taskQz.Id.ObjToString(),
                jobGroup = taskQz.JobGroup,
                triggerId = "",
                triggerGroup = "",
                triggerStatus = "不存在"
            } };
            JobKey jobKey = new JobKey(taskQz.Id.ToString(), taskQz.JobGroup);
            IJobDetail job = await this._scheduler.Result.GetJobDetail(jobKey);
            if (job == null)
            {
                return noTask;
            }
            //info.Append(string.Format("任务ID:{0}\r\n任务名称:{1}\r\n", job.Key.Name, job.Description)); 
            var triggers = await this._scheduler.Result.GetTriggersOfJob(jobKey);
            if (triggers == null || triggers.Count == 0)
            {
                return noTask;
            }
            foreach (var trigger in triggers)
            {
                var triggerStaus = await this._scheduler.Result.GetTriggerState(trigger.Key);
                string state = GetTriggerState(triggerStaus.ObjToString());
                ls.Add(new TaskInfoDto
                {
                    jobId = job.Key.Name,
                    jobGroup = job.Key.Group,
                    triggerId = trigger.Key.Name,
                    triggerGroup = trigger.Key.Group,
                    triggerStatus = state
                });
                //info.Append(string.Format("触发器ID:{0}\r\n触发器名称:{1}\r\n状态:{2}\r\n", item.Key.Name, item.Description, state));

            }
            return ls;
        }

        public string GetTriggerState(string key)
        {
            string state = null;
            if (key != null)
            {
                key = key.ToUpper();
            }

            switch (key)
            {
                case "1":
                    state = "暂停";
                    break;
                case "2":
                    state = "完成";
                    break;
                case "3":
                    state = "出错";
                    break;
                case "4":
                    state = "阻塞";
                    break;
                case "0":
                    state = "正常";
                    break;
                case "-1":
                    state = "不存在";
                    break;
                case "BLOCKED":
                    state = "阻塞";
                    break;
                case "COMPLETE":
                    state = "完成";
                    break;
                case "ERROR":
                    state = "出错";
                    break;
                case "NONE":
                    state = "不存在";
                    break;
                case "NORMAL":
                    state = "正常";
                    break;
                case "PAUSED":
                    state = "暂停";
                    break;
            }
            return state;
        }

        #endregion


        #region 创建触发器帮助方法

        /// <summary>
        /// 创建SimpleTrigger触发器（简单触发器）
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <param name="starRunTime"></param>
        /// <param name="endRunTime"></param>
        /// <returns></returns>
        private ITrigger CreateSimpleTrigger(TaskQz taskQz)
        {
            if (taskQz.CycleRunTimes > 0)
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQz.Id.ToString(), taskQz.JobGroup)
                .StartAt(taskQz.BeginTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQz.IntervalSecond)
                    .WithRepeatCount(taskQz.CycleRunTimes - 1))
                .EndAt(taskQz.EndTime.Value)
                .Build();
                return trigger;
            }
            else
            {
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskQz.Id.ToString(), taskQz.JobGroup)
                .StartAt(taskQz.BeginTime.Value)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(taskQz.IntervalSecond)
                    .RepeatForever()
                )
                .EndAt(taskQz.EndTime.Value)
                .Build();
                return trigger;
            }
            // 触发作业立即运行，然后每10秒重复一次，无限循环

        }
        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private ITrigger CreateCronTrigger(TaskQz taskQz)
        {
            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(taskQz.Id.ToString(), taskQz.JobGroup)
                   .StartAt(taskQz.BeginTime.Value)//开始时间
                   .EndAt(taskQz.EndTime.Value)//结束数据
                   .WithCronSchedule(taskQz.Cron)//指定cron表达式
                   .ForJob(taskQz.Id.ToString(), taskQz.JobGroup)//作业名称
                   .Build();
        }
        #endregion

    }
}
