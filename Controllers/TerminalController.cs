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

        //[ServiceFilter(typeof(ClientIdCheckFilter))]
        [AllowAnonymous]
        [HttpPost("eventfromterminal")]
        public IActionResult EventFromTerminal([FromBody]Event eventParam)
        {
            if (eventParam != null)
            {
                _terminalService.EventFromTerminal(eventParam);
            }
            return Ok();
        }

        //[ServiceFilter(typeof(ClientIdCheckFilter))]
        [AllowAnonymous]
        [HttpPost("depositfromterminal")]
        public IActionResult DepositFromTerminal([FromBody]DepositFromTerminal deposit)
        {
            bool ok = _terminalService.DepositFromTerminal(deposit);
            if(ok)
                return Ok("ok");
            else
                return Ok("error");
        }

        //[ServiceFilter(typeof(ClientIdCheckFilter))]
        [AllowAnonymous]
        [HttpPost("cashboxfromterminal")]
        public IActionResult CashBoxFromTerminal([FromBody]List<CashBox> lBox)
        {
            bool ok = _terminalService.CashBoxFromTerminal(lBox);
            if (ok)
                return Ok("ok");
            else
                return Ok("error");
        }

        //[ServiceFilter(typeof(ClientIdCheckFilter))]
        [AllowAnonymous]
        [HttpPost("withdrawfromterminal")]
        public IActionResult WithdrawFromTerminal([FromBody]CashBox box)
        {
            bool ok = _terminalService.WithdrawFromTerminal(box);
            if (ok)
                return Ok("ok");
            else
                return Ok("error");
        }

    }
    
}