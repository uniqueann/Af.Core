using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Extensions
{
    public static class AppConfigSetup
    {
        public static void AddAppConfigSetup(this IServiceCollection services, IWebHostEnvironment env)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.app(new string[] { "Startup","AppConfigConsole","Enabled" }).ObjToBool())
            {
                if (env.IsDevelopment())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Console.OutputEncoding = Encoding.GetEncoding("GB2312");
                }

                Console.WriteLine("********** Af.Core Config Set **********");

                ConsoleHelper.WriteSuccessLine($"Current environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

                // Redis缓存 AOP
                if (!Appsettings.app(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ObjToBool())
                {
                    Console.WriteLine("Redis Caching AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("Redis Caching AOP: True");
                }

                // 内存缓存 AOP
                if (!Appsettings.app(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ObjToBool())
                {
                    Console.WriteLine("Memory Caching AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("Memory Caching AOP: True");
                }

                // 服务日志 AOP
                if (!Appsettings.app(new string[] { "AppSettings", "LogAOP", "Enabled" }).ObjToBool())
                {
                    Console.WriteLine("Log AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("Log AOP: True");
                }

                // 事务 AOP
                if (!Appsettings.app(new string[] { "AppSettings", "TranAOP", "Enabled" }).ObjToBool())
                {
                    Console.WriteLine("Tran AOP: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("Tran AOP: True");
                }

                // 多库
                if (!Appsettings.app(new string[] { "MultiDBEnabled" }).ObjToBool())
                {
                    Console.WriteLine("MultiDBEnabled: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("Log AOP: True");
                }

                // 读写分离
                if (!Appsettings.app(new string[] { "CQRSEnabled" }).ObjToBool())
                {
                    Console.WriteLine("CQRSEnabled: False");
                }
                else
                {
                    ConsoleHelper.WriteSuccessLine("CQRSEnabled: True");
                }

                Console.WriteLine();
            }
        }
    }
}
