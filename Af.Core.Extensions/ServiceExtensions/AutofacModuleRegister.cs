using System;
using System.Collections.Generic;
using System.IO;
using Af.Core.Common.Helper;
using Autofac;
using log4net;

namespace Af.Core.Extensions
{
    public class AutofacModuleRegister : Autofac.Module
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AutofacModuleRegister));

        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;
            var servicesDllFile = Path.Combine(basePath, "Af.Core.Services.dll");
            var repositoryDllFile = Path.Combine(basePath, "Af.Core.Repository.dll");

            if (!(File.Exists(servicesDllFile) && File.Exists(repositoryDllFile)))
            {
                var msg = "Repository.dll和service.dll 丢失，因为项目解耦了，所以需要先F6编译，再F5运行，请检查 bin 文件夹，并拷贝。";
                log.Error(msg);
                throw new Exception(msg);
            }

            // AOP开关
            //var typeList = new List<Type>();

            //if (Appsettings.app(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ObjToBool())
            //{
            //    typeList.Add(typeof(RedisCacheAOP));
            //}
            //if (Appsettings.app(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ObjToBool())
            //{
            //    typeList.Add(typeof(CacheAOP));
            //}
            //if (Appsettings.app(new string[] { "AppSettings", "LogAOP", "Enabled" }).ObjToBool())
            //{
            //    typeList.Add(typeof(LogAOP));
            //}
        }
    }
}
