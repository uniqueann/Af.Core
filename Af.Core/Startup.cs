using Af.Core.AOP;
using Af.Core.AutoMapper;
using Af.Core.Common.Helper;
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
using System.IO;
using System.Reflection;
using System.Text;

namespace Af.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            // 直接注册某个类和接口
            //builder.RegisterType<UserServices>().As<IUserServices>();
            builder.RegisterType<CacheAOP>();
            builder.RegisterType<LogAOP>();


            // 注册要通过反射注册的组件
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var basePath = Environment.CurrentDirectory;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "V1",
                    Title = "Af.Core 接口文档--NetCore 3.1",
                    Description = "Af.Core Http API V1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Af.Core", Email = "uniqueann@163.com" },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "Af.Core", Url = new Uri("https://www.xxx.com") }

                });
                c.OrderActionsBy(o => o.RelativePath);
                // 注释 xml 
                var xmlPath = Path.Combine(basePath, "af.core.xml");
                c.IncludeXmlComments(xmlPath, true);
                var xmlModelPath = Path.Combine(basePath, "af.core.model.xml");
                c.IncludeXmlComments(xmlModelPath);

                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            // 注入缓存
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory => {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

            //1.基于角色的api授权
            //1.1 授权 无需配置服务，只需要在api层的controller上边增加特性即可。只能是角色的
            //1.2 认证 然后在下边的configure里，配置中间件即可：app.UseMiddleware<JwtTokenAuth>(); 
            //      此方法无法验证过期时间，如果需要验证过期时间，则需要下边的第三种方法--官方认证

            //2.基于策略的授权
            //2.1 授权 无需配置服务，api层controller上增加特性即可，可以写多个roles
            //      [Authorize(Policy="Admin")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Client", "Admin").Build()); ;
            });
            //2.2 认证 然后在下边的configure里，配置中间件即可

            //读取配置文件
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
                    ValidIssuer = audienceConfig["Issuer"],//发行人
                    ValidateAudience = true,
                    ValidAudience = audienceConfig["Audience"],//订阅人
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
                //路径配置 设置为空，表示直接在根域名访问该文件
                c.RoutePrefix = "";
            });

            app.UseRouting();
            app.UseCors();
            //先开启认证
            app.UseAuthentication();
            //然后是授权中间件
            app.UseAuthorization();
            //开启异常中间件 要放到最后



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
