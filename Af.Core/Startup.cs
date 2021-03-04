using Af.Core.AutoMapper;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.Common.Hubs;
using Af.Core.Common.LogHelper;
using Af.Core.Extensions;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Text;

namespace Af.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            
            services.AddSingleton(new Appsettings(Configuration));
            services.AddSingleton(new LogLock(Env.ContentRootPath));

            Permissions.IsUseIds4 = Appsettings.app(new string[] { "Startup", "IdentityServer4", "Enabled" }).ObjToBool();
            //
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMemoryCacheSetup();
            services.AddRedisCacheSetup();            
            services.AddAutoMapperSetup();
            services.AddSqlsugarSetup();
            services.AddCorsSetup();
            services.AddSwaggerSetup();
            services.AddJobSetup();
            services.AddHttpContextSetup();
            services.AddAuthorizationSetup();

            if (Permissions.IsUseIds4)
            {

            }
            else
            {
                services.AddAuthentication_JWTSetup();
            }

            services.AddCors(c=> {
                c.AddPolicy("LimitRequest",policy=> {
                    policy.WithOrigins("http://localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddSignalR();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ������Ӧ��־��¼�м��
            app.UseReqRespLogMidd();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Af.Core V1");
                c.RoutePrefix = "";
            });

            // cors��������
            app.UseCors(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }));
            
            //��תhttps
            //app.UseHttpsRedirection();

            //ʹ�þ�̬�ļ�
            app.UseStaticFiles();

            //ʹ��cookie
            app.UseCookiePolicy();

            //������
            app.UseStatusCodePages();
            
            //routing
            app.UseRouting();
            
            //������֤
            app.UseAuthentication();
            
            //��Ȩ�м��
            app.UseAuthorization();
            
            //�쳣�����м��
            app.UseExceptionHandlerMidd();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Role}/{action=Index}/{id?}");
                //endpoints.MapHub<ChatHub>("/api/chatHub");
            });

           
        }
    }
}
