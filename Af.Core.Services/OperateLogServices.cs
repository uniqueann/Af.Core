using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class OperateLogServices : BaseServices<OperateLog>, IOperateLogServices
    {
        private readonly IBaseRepository<OperateLog> _dal;

        public OperateLogServices(IBaseRepository<OperateLog> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
