using Af.Core.Common.Converter;
using Af.Core.Common.LogHelper;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Tasks
{
    public class Job_OperateLog_Quartz : JobBase, IJob
    {
        private readonly IOperateLogServices _operateLogServices;
        private readonly IWebHostEnvironment _env;

        public Job_OperateLog_Quartz(IOperateLogServices operateLogServices, IWebHostEnvironment env,ITaskQzServices taskQzServices):base(taskQzServices)
        {
            _operateLogServices = operateLogServices;
            _env = env;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;

            var executeLog = await ExecuteJob(context, async () => await Run(context, jobId.ObjToInt()));
        }

        public async Task Run(IJobExecutionContext context, int jobId)
        {
            var logs = new List<LogInfo>();
            var logContent = LogLock.ReadLog(Path.Combine(_env.ContentRootPath, "Log"), $"SqlLog_{DateTime.Now.ToString("yyyyMMdd")}.log", Encoding.UTF8);

            if (!string.IsNullOrEmpty(logContent))
            {
                logs = logContent.Split("--------------------------------")
                    .Where(a => !string.IsNullOrEmpty(a) && a != "\n" && a != "\r\n")
                    .Select(a => new LogInfo
                    {
                        Datetime = (a.Split('|')[0]).Split(',')[0].ObjToDate(),
                        Content = a.Split('|')[1]?.Replace("\r\n", "<br>"),
                        LogColor = "",
                        Import = 10
                    }).ToList();
            }

            var filterDatetime = DateTime.Now.AddHours(-1);
            logs = logs.Where(a=>a.Datetime>=filterDatetime).ToList();

            var operateLogs = new List<OperateLog>();
            logs.ForEach(item=> {
                operateLogs.Add(new OperateLog { 
                    LogTime = item.Datetime,
                    Description = item.Content,
                    IPAddress = item.IP,
                    UserId = 0,
                    IsDeleted = false
                });
            });

            if (operateLogs.Count>0)
            {
                var logIds = await _operateLogServices.Add(operateLogs);
            }

            if (jobId>0)
            {
                var model = await _taskQzServices.QueryById(jobId);
                if (model!=null)
                {
                    var list = await _operateLogServices.Query(a => a.IsDeleted == false);
                    model.RunTimes += 1;
                    var separator = "<br>";
                    model.Remark = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] " +
                        $"execution JobId: {context.JobDetail.Key.Name},Group: {context.JobDetail.Key.Group} successful";
                }
            }
        }
    }
}
