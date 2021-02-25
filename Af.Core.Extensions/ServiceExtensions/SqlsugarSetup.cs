using Af.Core.Common.DB;
using Af.Core.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using System.Threading.Tasks;
using StackExchange.Profiling;
using Af.Core.Common.LogHelper;
using Af.Core.Common.Converter;

namespace Af.Core.Extensions
{
    public static class SqlsugarSetup
    {
        /// <summary>
        /// Sqlsugar启动服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //默认添加主数据库连接
            MainDb.CurrentConnId = Appsettings.app(new string[] { "MainDB" });
            //多个连接对象注入服务 必须采用scope 因为有事务
            services.AddScoped<ISqlSugarClient>(a=> 
            {
                // 连接字符串
                var listConfig = new List<ConnectionConfig>();

                //从库
                var listConfigSlave = new List<SlaveConnectionConfig>();
                BaseDBConfig.MultiConnectionString.slaveDbs.ForEach(item =>
                {
                    listConfigSlave.Add(new SlaveConnectionConfig
                    {
                        HitRate = item.HiRate,
                        ConnectionString = item.Connection
                    });
                });

                BaseDBConfig.MultiConnectionString.allDbs.ForEach(item=>
                {
                    listConfig.Add(new ConnectionConfig {
                        ConfigId = item.ConnId.ObjToString().ToLower(),
                        ConnectionString = item.Connection,
                        DbType = (DbType)item.DbType,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = false,
                        AopEvents = new AopEvents {
                            OnLogExecuting = (sql, p) =>
                              {
                                  if (Appsettings.app(new string[] { "AppSettings", "SqlAOP", "Enabled" }).ObjToBool())
                                  {
                                      Parallel.For(0, 1, e =>
                                        {
                                            MiniProfiler.Current.CustomTiming("SQL: ", GetParas(p) + "【SQL语句】:" + sql);
                                            LogLock.OutSql2Log("SqlLog", new string[] { GetParas(p), "【SQL语句】:" + sql });
                                        });
                                  }
                              }
                        },
                        MoreSettings = new ConnMoreSettings
                        {
                            IsWithNoLockQuery = true,
                            IsAutoRemoveDataCache = true
                        },
                        // 从库
                        SlaveConnectionConfigs = listConfigSlave,
                        // 自定义特性
                        ConfigureExternalServices = new ConfigureExternalServices {
                            EntityService = (property, column) =>
                            {
                                if (column.IsPrimarykey && property.PropertyType == typeof(int))
                                {
                                    column.IsIdentity = true;
                                }
                            }
                        },
                        InitKeyType = InitKeyType.Attribute
                    });
                });

                return new SqlSugarClient(listConfig);
            });
        }

        private static string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value}\n";
            }

            return key;
        }
    }
}
