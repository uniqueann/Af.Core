using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.LogHelper;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Extensions
{
    /// <summary>
    /// IP 请求记录 中间件
    /// </summary>
    public class IpLogMidd
    {
        private readonly RequestDelegate _next;
        private readonly ILog _log = LogManager.GetLogger(typeof(IpLogMidd));

        public IpLogMidd(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.app(new string[] { "Middleware", "IpLog", "Enabled" }).ObjToBool())
            {
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();

                    try
                    {
                        var req = context.Request;
                        var reqInfo = JsonConvert.SerializeObject(new RequestInfo { 
                            Ip = GetClientIP(context),
                            Url = req.Path.ObjToString().TrimEnd('/').ToLower(),
                            Datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Date = DateTime.Now.ToString("yyyy-MM-dd"),
                            Week = GetWeek()
                        });

                        if (!string.IsNullOrEmpty(reqInfo))
                        {
                            Parallel.For(0, 1, e =>
                            {
                                LogLock.OutSql2Log("RequestIpInfoLog",new string[] { reqInfo+"," },false);
                            });

                            req.Body.Position = 0;
                        }

                        await _next(context);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
        private string GetWeek()
        {
            string week = string.Empty;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = "周一";
                    break;
                case DayOfWeek.Tuesday:
                    week = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    week = "周三";
                    break;
                case DayOfWeek.Thursday:
                    week = "周四";
                    break;
                case DayOfWeek.Friday:
                    week = "周五";
                    break;
                case DayOfWeek.Saturday:
                    week = "周六";
                    break;
                case DayOfWeek.Sunday:
                    week = "周日";
                    break;
                default:
                    week = "N/A";
                    break;
            }
            return week;
        }
        public static string GetClientIP(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ObjToString();
            }
            return ip;
        }
    }
}
