using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Af.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c=> 
            {
                c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo {
                    Version = "V1",
                    Title = "Af.Core 接口文档--NetCore 3.1",
                    Description = "Af.Core Http API V1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Af.Core", Email="uniqueann@163.com" },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "Af.Core", Url = new Uri("https://www.xxx.com") }

                });
                c.OrderActionsBy(o => o.RelativePath);
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c=> {
                c.SwaggerEndpoint("/swagger/V1/swagger.json","Af.Core V1");
                //路径配置 设置为空，表示直接在根域名访问该文件
                c.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
