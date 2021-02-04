using Af.Core.Common.Converter;
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
        IBaseRepository<Employee> _dal;
        IMapper _mapper;

        public EmployeeServices(IBaseRepository<Employee> dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
            base.BaseDal = dal;
        }

        public async Task<EmployeeViewModel> GetEmployee(int id)
        {
            var model = await _dal.QueryById(id);
            EmployeeViewModel vModel = _mapper.Map<EmployeeViewModel>(model);
            return vModel;
        }

        public async Task<PageModel<EmployeeViewModel>> GetEmployeeList(int pageIndex, int pageSize, string employeeName)
        {
            var employeeList = await _dal.QueryPage(a => a.EmployeeName.Contains(employeeName.ObjToString()), pageIndex, pageSize, "UpdTime desc");
            PageModel<EmployeeViewModel> vModel = _mapper.Map<PageModel<EmployeeViewModel>>(employeeList);
            return vModel;
        }
    }
}
