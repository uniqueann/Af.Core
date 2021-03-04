using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.HttpContextUser;
using Af.Core.Common.LogHelper;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Af.Core.Extensions
{
    public class RecordAccessLogMidd
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RecordAccessLogMidd> _logger;
        private readonly IUser _user;
        private Stopwatch _stopwatch;

        public RecordAccessLogMidd(RequestDelegate next, ILogger<RecordAccessLogMidd> logger, IUser user)
        {
            _next = next;
            _logger = logger;
            _user = user;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.app(new string[] { "Middleware", "RecordAccessLog", "Enabled" }).ObjToBool())
            {
                var api = context.Request.Path.Value.TrimEnd('/').ToLower();
                var ignoreApis = Appsettings.app(new string[] {"Middleware","RecordAccessLog","IgnoreApis" });

                if (api.Contains("api") && !ignoreApis.Contains(api))
                {
                    _stopwatch.Restart();
                    var userAccessModel = new UserAccessModel();

                    var req = context.Request;
                    userAccessModel.API = api;
                    userAccessModel.User = _user.Name;
                    userAccessModel.IP = IpLogMidd.GetClientIP(context);
                    userAccessModel.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    userAccessModel.RequestMethod = req.Method;
                    userAccessModel.Agent = req.Headers["User-Agent"].ObjToString();

                    var methodName = req.Method.ToLower();
                    if (methodName=="post" || methodName == "put")
                    {
                        //启用倒带功能 request.body就可以再次读取到
                        req.EnableBuffering();

                        var stream = req.Body;
                        byte[] buffer = new byte[req.ContentLength.Value];
                        stream.Read(buffer, 0 ,buffer.Length);
                        userAccessModel.RequestData = Encoding.UTF8.GetString(buffer);

                        req.Body.Position = 0;
                    }
                    else if (methodName=="get" || methodName=="delete")
                    {
                        userAccessModel.RequestData = HttpUtility.UrlDecode(req.QueryString.ObjToString(),Encoding.UTF8);
                    }

                    // response.body内容
                    var originalBodyStream = context.Response.Body;
                    using (var respBody = new MemoryStream())
                    {
                        context.Response.Body = respBody;

                        await _next(context);

                        var respBodyData = await GetResponse(context.Response);

                        await respBody.CopyToAsync(originalBodyStream);
                    }

                    //存入日志
                    context.Response.OnCompleted(() =>
                    {
                        _stopwatch.Stop();

                        userAccessModel.OPTime = _stopwatch.ElapsedMilliseconds + "ms";
                        //log记录
                        var reqInfo = JsonConvert.SerializeObject(userAccessModel);
                        Parallel.For(0, 1, e =>
                        {
                            LogLock.OutSql2Log("RecordAccessLog", new string[] { reqInfo + "," }, false);
                        });
                        return Task.CompletedTask;
                    });
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

        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }

    public class UserAccessModel
    {
        public string User { get; set; }
        public string IP { get; set; }
        public string API { get; set; }
        public string BeginTime { get; set; }
        public string OPTime { get; set; }
        public string RequestMethod { get; set; }
        public string RequestData { get; set; }
        public string Agent { get; set; }

    }
}
