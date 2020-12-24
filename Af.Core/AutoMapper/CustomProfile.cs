using Af.Core.Model.Models;
using Af.Core.Model.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Af.Core.AutoMapper
{
    public class CustomProfile: Profile
    {
        public CustomProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<PageModel<User>, PageModel<UserViewModel>>();
        }
    }
}
