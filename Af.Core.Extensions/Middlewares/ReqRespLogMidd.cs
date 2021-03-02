using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.LogHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Af.Core.Extensions
{
    /// <summary>
    /// 记录请求响应中间件
    /// </summary>
    public class ReqRespLogMidd
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ReqRespLogMidd> _logger;

        public ReqRespLogMidd(RequestDelegate next, ILogger<ReqRespLogMidd> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.app(new string[] { "Middleware", "RequestResponseLog", "Enabled" }).ObjToBool())
            {
                // 只记录api
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();
                    var originalBody = context.Response.Body;
                    try
                    {
                        //存储请求数据
                        await RequestDataLog(context);

                        using (var ms = new MemoryStream())
                        {
                            context.Response.Body = ms;
                            await _next(context);
                            //存储响应数据
                            ResponseDataLog(context.Response,ms);

                            ms.Position = 0;
                            await ms.CopyToAsync(originalBody);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message+" "+ex.InnerException);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
                else
                {
                    _next(context);
                }
            }
            else
            {
                _next(context);
            }
        }

        private async Task RequestDataLog(HttpContext context)
        {
            var request = context.Request;
            var sr = new StreamReader(request.Body);

            var content = $" QueryData:{request.Path + request.QueryString}\r\n BodyData:{await sr.ReadToEndAsync()}";

            if (!string.IsNullOrEmpty(content))
            {
                Parallel.For(0, 1, e =>
                {
                    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Request Data:", content });

                });

                request.Body.Position = 0;
            }
        }

        private void ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var responseBody = new StreamReader(ms).ReadToEnd();

            // 去除 Html
            var reg = "<[^>]+>";
            var isHtml = Regex.IsMatch(responseBody, reg);

            if (!string.IsNullOrEmpty(responseBody))
            {
                Parallel.For(0, 1, e =>
                {
                    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Response Data:", responseBody });

                });
            }
        }
    }
}
