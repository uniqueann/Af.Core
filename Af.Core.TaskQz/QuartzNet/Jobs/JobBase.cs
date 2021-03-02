using Af.Core.Common.Converter;
using Af.Core.IServices;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Tasks
{
    public class JobBase
    {
        public readonly ITaskQzServices _taskQzServices;

        public JobBase(ITaskQzServices taskQzServices)
        {
            _taskQzServices = taskQzServices;
        }

        public async Task<string> ExecuteJob(IJobExecutionContext context,Func<Task> func)
        {
            //job时间
            Stopwatch stopwatch = new Stopwatch();
            //job id
            var jobId = context.JobDetail.Key.Name.ObjToInt();
            var groupName = context.JobDetail.Key.Group;
            //日志
            var jobHistory = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}],start execution JobId: {jobId}, GroupName: {groupName}";
            // 用时
            double taskSecounds = 0;
            try
            {
                stopwatch.Start();
                await func();
                stopwatch.Stop();
                jobHistory += $",succeed"; 
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                //true  是立即重新执行任务 
                e2.RefireImmediately = true;
                //SerilogServer.WriteErrorLog(context.Trigger.Key.Name.Replace("-", ""), $"{context.Trigger.Key.Name}任务运行异常", ex);

                jobHistory += $",[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][failed: {ex.Message}]";
            }
            finally
            {
                taskSecounds = Math.Round(stopwatch.Elapsed.TotalSeconds, 3);
                jobHistory += $"";
                var model = await _taskQzServices.QueryById(jobId);
                if (model!= null)
                {
                    model.RunTimes += 1;
                    var separator = "<br>";
                    model.Remark = $"{jobHistory}{separator}";
                    await _taskQzServices.Update(model);
                }
            }

            Console.Out.WriteLine(jobHistory);
            return jobHistory;
        }
    }
}
