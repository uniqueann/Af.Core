using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services.BASE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Services
{
    public class TaskQzServices:BaseServices<TaskQz>,ITaskQzServices
    {
        IBaseRepository<TaskQz> _dal;

        public TaskQzServices(IBaseRepository<TaskQz> dal)
        {
            _dal = dal;
            base.BaseDal = dal;
        }
    }
}
