using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.IServices;
using Af.Core.Model.Models;
using Af.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("{id}", Name ="Get")]
        public List<User> Get(int id)
        {
            IUserServices services = new UserServices();
            return services.Query(a => a.UserId == id);
        }
    }
}
