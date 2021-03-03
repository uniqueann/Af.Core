using Af.Core.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Extensions
{
    public static class RedisCacheSetup
    {
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();
            //配置启动redis服务
            services.AddSingleton<ConnectionMultiplexer>(a =>
            {
                var redisConfig = Appsettings.app(new string[] { "Redis", "ConnectionString" });
                var config = ConfigurationOptions.Parse(redisConfig, true);
                config.ResolveDns = false;
                return ConnectionMultiplexer.Connect(config);
            });
        }
    }
}
