using Af.Core.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Extensions
{
    public static  class JobSetup
    {
        public static void AddJobSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddScoped<Job_OperateLog_Quartz>();
            services.AddSingleton<ISchedulerCenter, SchedulerCenterServer>();

        }
    }
}
