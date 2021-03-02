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
            /***
             * var config2 = new MapperConfiguration(
                    cfg => cfg.CreateMap<SourceUser, DestUser2>()
                        .ForMember(d => d.DestName, opt => opt.MapFrom(s => s.Name))    //指定字段一一对应
                        .ForMember(d => d.Birthday, opt => opt.MapFrom(src => src.Birthday.ToString("yy-MM-dd HH:mm")))//指定字段，并转化指定的格式
                        .ForMember(d => d.Age, opt => opt.Condition(src => src.Age > 5))//条件赋值
                        .ForMember(d => d.A1, opt => opt.Ignore())//忽略该字段，不给该字段赋值
                        .ForMember(d => d.A1, opt => opt.NullSubstitute("Default Value"))//如果源字段值为空，则赋值为 Default Value
                        .ForMember(d => d.A1, opt => opt.MapFrom(src => src.Name + src.Age * 3 + src.Birthday.ToString("d"))));//可以自己随意组合赋值
                var mapper2 = config2.CreateMapper();
             */
            CreateMap<User, UserViewModel>();
            CreateMap<PageModel<User>, PageModel<UserViewModel>>();
            CreateMap<Employee, EmployeeViewModel>().ForMember(d=>d.EmployeeId,opt=>opt.MapFrom(s=>s.Id));
        }
    }
}
