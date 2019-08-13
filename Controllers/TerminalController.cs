using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wscore.Entities;
using wscore.Helpers;
using wscore.Services;

namespace wscore.Controllers
{

    [Produces("application/json")]
    [Route("terminal")]
    public class TerminalController : Controller
    {
        private ITerminalService _terminalService;

        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First().Value); 
        }

        protected string GetUserName()
        {
            return this.User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
        }

        public TerminalController(ITerminalService terminalService)
        {
            _terminalService = terminalService;
        }


        /*
        [Authorize(Roles = "Admin,User")]
        or
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "User")]
        */

        //[AllowAnonymous]
        [ServiceFilter(typeof(ClientIdCheckFilter))]
        [HttpGet("deposit")]
        public IActionResult DepositTerminal(string deposit)
        {
            _terminalService.DepositTerminal(deposit);
            return Ok();
        }

        //[AllowAnonymous]
        [ServiceFilter(typeof(ClientIdCheckFilter))]
        [HttpGet("event")]
        public IActionResult EventTerminal(string terminalId, string eventId)
        {
            _terminalService.EventTerminal(int.Parse(terminalId), int.Parse(eventId));
            return Ok();
        }

    }
    
}