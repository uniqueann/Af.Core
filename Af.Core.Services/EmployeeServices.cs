using Af.Core.Common.Convert;
using Af.Core.IRepository;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Af.Core.Services.BASE;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Af.Core.Services
{
    public class EmployeeServices:BaseServices<Employee>,IEmployeeServices
    {
        IEmployeeRepository _empDal;
        IMapper _mapper;

        public EmployeeServices(IBaseRepository<Employee> balDal, IEmployeeRepository empDal, IMapper mapper):base(balDal)
        {
            _empDal = empDal;
            _mapper = mapper;
        }

        public async Task<EmployeeViewModel> GetEmployee(int id)
        {
            var model = await _empDal.QueryById(id);
            EmployeeViewModel vModel = _mapper.Map<EmployeeViewModel>(model);
            return vModel;
        }

        public async Task<PageModel<EmployeeViewModel>> GetEmployeeList(int pageIndex, int pageSize, string employeeName)
        {
            var employeeList = await _empDal.QueryPage(a => a.EmployeeName.Contains(employeeName.ObjToString()), pageIndex, pageSize, "UpdTime desc");
            PageModel<EmployeeViewModel> vModel = _mapper.Map<PageModel<EmployeeViewModel>>(employeeList);
            return vModel;
        }
    }
}
