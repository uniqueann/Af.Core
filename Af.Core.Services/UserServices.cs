using Af.Core.Common.Convert;
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
        IUserRepository _userDal;
        IMapper _mapper;
        public UserServices(IBaseRepository<User> balDal,IUserRepository userDal,IMapper mapper) : base(balDal)
        {
            _userDal = userDal;
            _mapper = mapper;
        }


        public async Task<UserViewModel> GetUser(int id)
        {
            var model = await _userDal.QueryById(id);
            UserViewModel vModel = _mapper.Map<UserViewModel>(model);
            return vModel;
        }

        public async Task<PageModel<UserViewModel>> GetUserList(int pageIndex, int pageSize, string userName)
        {
            var userList = await _userDal.QueryPage(a => a.IsEnable && a.UserName.Contains(userName.ObjToString()), pageIndex, pageSize, "CreateTime desc");
            PageModel<UserViewModel> vModel = _mapper.Map<PageModel<UserViewModel>>(userList);
            return vModel;
        }
    }
}
