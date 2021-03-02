using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Af.Core.Common.Hubs;
using Af.Core.Common.LogHelper;
using Af.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Af.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class MonitorController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MonitorController> _logger;

        public MonitorController(IHubContext<ChatHub> hubContext, IWebHostEnvironment env, ILogger<MonitorController> logger)
        {
            _hubContext = hubContext;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public MessageModel<List<LogInfo>> Get()
        {
            _hubContext.Clients.All.SendAsync("ReceiveMessage", LogLock.GetLogData()).Wait();

            return new MessageModel<List<LogInfo>> { 
                success = true,
                response = null,
                msg = "获取成功"
            };
        }
    }
}
