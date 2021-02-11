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

            // ֱ��ע��ĳ����ͽӿ�
            //builder.RegisterType<UserServices>().As<IUserServices>();
            builder.RegisterType<CacheAOP>();
            builder.RegisterType<LogAOP>();



            // ע��Ҫͨ������ע������
            var servicesDllFile = Path.Combine(basePath, "Af.Core.Services.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);

            var respositoryDllFile = Path.Combine(basePath, "Af.Core.Repository.dll");
            var assemblysRepository = Assembly.LoadFrom(respositoryDllFile);

            builder.RegisterAssemblyTypes(assemblysServices,assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeList.ToArray()); 
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // ע�뻺��
            services.AddSingleton(new Appsettings(Configuration));
            services.AddSingleton(new LogLock(Env.ContentRootPath));

            Permissions.IsUseIds4 = Appsettings.app(new string[] { "Startup", "IdentityServer4", "Enabled" }).ObjToBool();
            //ȷ������֤���ķ��ص�ClaimType�������ģ�������Mapӳ��
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory => {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            services.AddSqlsugarSetup();
            services.AddSwaggerSetup();

            //1.���ڽ�ɫ��api��Ȩ
            //1.1 ��Ȩ �������÷���ֻ��Ҫ��api���controller�ϱ��������Լ��ɡ�ֻ���ǽ�ɫ��
            //1.2 ��֤ Ȼ�����±ߵ�configure������м�����ɣ�app.UseMiddleware<JwtTokenAuth>(); 
            //      �˷����޷���֤����ʱ�䣬�����Ҫ��֤����ʱ�䣬����Ҫ�±ߵĵ����ַ���--�ٷ���֤

            //2.���ڲ��Ե���Ȩ
            //2.1 ��Ȩ �������÷���api��controller���������Լ��ɣ�����д���roles
            //      [Authorize(Policy="Admin")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Client", "Admin").Build()); ;
            });
            //2.2 ��֤ Ȼ�����±ߵ�configure������м������

            //��ȡ�����ļ�
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = audienceConfig["Issuer"],//������
                    ValidateAudience = true,
                    ValidAudience = audienceConfig["Audience"],//������
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
            });

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
                //·������ ����Ϊ�գ���ʾֱ���ڸ��������ʸ��ļ�
                c.RoutePrefix = "";
            });

            app.UseRouting();
            app.UseCors();
            //�ȿ�����֤
            app.UseAuthentication();
            //Ȼ������Ȩ�м��
            app.UseAuthorization();
            //�����쳣�м�� Ҫ�ŵ����


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Role}/{action=Index}/{id?}");
            });
        }
    }
}
