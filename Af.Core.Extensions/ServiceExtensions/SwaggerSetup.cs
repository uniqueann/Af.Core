using Af.Core.Common;
using Af.Core.Common.Helper;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Af.Core.Extensions
{
    public static class SwaggerSetup
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SwaggerSetup));

        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            var apiName = Appsettings.app(new string[] { "Startup", "ApiName" });
            services.AddSwaggerGen(c =>
            {
                // 遍历版本
                typeof(CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"{apiName} 接口文档--{RuntimeInformation.FrameworkDescription}",
                        Description = $"{apiName} HTTP API {version}",
                        Contact = new OpenApiContact { Name = apiName, Email = "xxx@163.com",Url = new Uri("http://xxx.com") },
                        License = new OpenApiLicense { Name = apiName+" 官方文档", Url = new Uri("http://xxx.com") }
                    });
                    c.OrderActionsBy(o=>o.RelativePath);
                });

                try
                {
                    var xmlPath = Path.Combine(basePath,"Af.Core.xml");
                    c.IncludeXmlComments(xmlPath, true);

                    var modelXmlPath = Path.Combine(basePath, "Af.Core.Model.xml");
                    c.IncludeXmlComments(modelXmlPath, true);
                }
                catch (Exception ex)
                {
                    log.Error($"Af.Core.xml和Af.Core.Model.xml 丢失,请检查并拷贝。\n{ex.Message}");
                }

                // 开启权限小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 添加token到header 
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                if (Permissions.IsUseIds4)
                {

                }
                else
                {
                    // jwt bearer认证 必须oauth2
                    c.AddSecurityDefinition("oauth2",new OpenApiSecurityScheme {
                        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                }
            });

        }
    }

    /// <summary>
    /// 自定义版本
    /// </summary>
    public class CustomApiVersion
    {
        /// <summary>
        /// Api接口版本 自定义
        /// </summary>
        public enum ApiVersions
        {
            /// <summary>
            /// V1 版本
            /// </summary>
            V1 = 1,
            /// <summary>
            /// V2 版本
            /// </summary>
            V2 = 2,
        }
    }
}
