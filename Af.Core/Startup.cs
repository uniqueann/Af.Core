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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var basePath = Environment.CurrentDirectory;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "V1",
                    Title = "Af.Core �ӿ��ĵ�--NetCore 3.1",
                    Description = "Af.Core Http API V1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Af.Core", Email = "uniqueann@163.com" },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "Af.Core", Url = new Uri("https://www.xxx.com") }

                });
                c.OrderActionsBy(o => o.RelativePath);
                // ע�� xml 
                var xmlPath = Path.Combine(basePath, "af.core.xml");
                c.IncludeXmlComments(xmlPath, true);
                var xmlModelPath = Path.Combine(basePath, "af.core.model.xml");
                c.IncludeXmlComments(xmlModelPath);

                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            // ע�뻺��
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory => {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();

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
                endpoints.MapControllers();
            });
        }
    }
}
