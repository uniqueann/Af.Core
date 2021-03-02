using Af.Core.IRepository;
using Af.Core.IRepository.UnitOfWork;
using Af.Core.Model.Models;
using Af.Core.Repository.BASE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Repository
{
    public class TaskQzRepository : BaseRepository<TaskQz>, ITaskQzRepository
    {
        public TaskQzRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
