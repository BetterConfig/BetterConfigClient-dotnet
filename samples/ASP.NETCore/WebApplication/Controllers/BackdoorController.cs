using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/backdoor")]
    public class BackdoorController : Controller
    {
        private readonly IBetterConfigClient betterConfigClient;

        public BackdoorController(IBetterConfigClient betterConfigClient)
        {
            this.betterConfigClient = betterConfigClient;
        }

        // GET: api/backdoor/betterconfigchanged
        [HttpGet]
        [Route("betterconfigchanged")]
        public void BetterConfigChanged()
        {
            this.betterConfigClient.ForceRefresh();
        }
    }
}