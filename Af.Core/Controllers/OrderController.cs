using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.IServices;
using Af.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Af.Core.Controllers
{
    /// <summary>
    /// 订单
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        ///// <summary>
        ///// 根据id获取指定订单
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet("{id}")]
        //[Authorize]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        [HttpGet]
        //[Authorize(Policy ="Admin")]
        public int Get(int id)
        {
            return 0;
        }
    }
}
