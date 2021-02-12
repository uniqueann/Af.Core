using System;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;

namespace Af.Core.Services
{
    public class ModuleServices : BaseServices<Module>, IModuleServices
    {
        IBaseRepository<Module> _dal;

        public ModuleServices(IBaseRepository<Module> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
