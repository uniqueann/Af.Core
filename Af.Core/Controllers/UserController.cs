using Af.Core.IServices;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpGet("{id}", Name = "Get")]
        public async Task<User> GetAsync(int id)
        {
            return await _userServices.QueryByID(id);
        }
    }
}
