using Af.Core.AOP;
using Af.Core.AutoMapper;
using Af.Core.Common;
using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
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
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var typeList = new List<Type>();

            if (Appsettings.app(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).ObjToBool())
            {
                typeList.Add(typeof(RedisCacheAOP));
            }
            if (Appsettings.app(new string[] { "AppSettings","MemoryCachingAOP","Enabled" }).ObjToBool())
            {
                typeList.Add(typeof(CacheAOP));
            }
            if (Appsettings.app(new string[] { "AppSettings", "LogAOP", "Enabled" }).ObjToBool())
            {
                typeList.Add(typeof(LogAOP));
            }

         
            //builder.RegisterType<UserServices>().As<IUserServices>();
            builder.RegisterType<CacheAOP>();
            builder.RegisterType<LogAOP>();


            var servicesDllFile = Path.Combine(basePath, "Af.Core.Services.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);

            var respositoryDllFile = Path.Combine(basePath, "Af.Core.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(respositoryDllFile);

            builder.RegisterAssemblyTypes(assemblysServices,assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .InterceptedBy(typeList.ToArray()); 
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

            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory => {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            services.AddSqlsugarSetup();
            services.AddCorsSetup();
            services.AddSwaggerSetup();
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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Af.Core V1");
                c.RoutePrefix = "";
            });

            app.UseRouting();
            app.UseCors(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }));

            app.UseAuthentication();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Role}/{action=Index}/{id?}");
            });
        }
    }
}
