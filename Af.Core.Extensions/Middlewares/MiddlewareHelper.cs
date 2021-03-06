﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Extensions
{
    public static class MiddlewareHelper
    {
        /// <summary>
        /// 记录请求响应日志中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReqRespLogMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ReqRespLogMidd>();
        }
        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlerMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMidd>();
        }

        /// <summary>
        /// ip请求记录中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpLoginMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IpLogMidd>();
        }

        /// <summary>
        /// 用户访问记录中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRecordAccessLogMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RecordAccessLogMidd>();
        }
    }
}
