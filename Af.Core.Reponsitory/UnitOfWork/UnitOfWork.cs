using Af.Core.IRepository.UnitOfWork;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;

namespace Af.Core.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(ISqlSugarClient sqlSugarClient, ILogger<UnitOfWork> logger)
        {
            _sqlSugarClient = sqlSugarClient;
            _logger = logger;
        }

        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran();
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                _logger.LogError($"{ex.Message}\r\n{ex.InnerException}");
            }
        }

        /// <summary>
        /// 获取唯一性DB
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetDbClient()
        {
            // 必须as 后边会用到切换数据库操作
            return _sqlSugarClient as SqlSugarClient;
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }
    }
}
