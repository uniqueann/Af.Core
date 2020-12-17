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

        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("{id}", Name ="Get")]
        public async Task<User> GetAsync(int id)
        {
            return await _userServices.QueryByID(id);
        }
    }
}
