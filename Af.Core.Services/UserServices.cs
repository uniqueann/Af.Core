using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using Af.Core.IRepository;
using Af.Core.IRepository.BASE;
using Af.Core.IServices;
using Af.Core.IServices.BASE;
using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using Af.Core.Repository;
using Af.Core.Services.BASE;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Af.Core.Services
{
    public class UserServices : BaseServices<User>, IUserServices
    {
        IBaseRepository<User> _dal;
        IMapper _mapper;
        public UserServices(IBaseRepository<User> dal,IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }


        public async Task<UserViewModel> GetUser(int id)
        {
            var model = await _dal.QueryById(id);
            UserViewModel vModel = _mapper.Map<UserViewModel>(model);
            return vModel;
        }

        public async Task<PageModel<UserViewModel>> GetUserList(int pageIndex, int pageSize, string userName)
        {
            var userList = await _dal.QueryPage(a => a.IsEnable && a.UserName.Contains(userName.ObjToString()), pageIndex, pageSize, "CreateTime desc");
            PageModel<UserViewModel> vModel = _mapper.Map<PageModel<UserViewModel>>(userList);
            return vModel;
        }
    }
}
