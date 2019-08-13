using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wscore.Entities;
using wscore.Helpers;
using wscore.Services;

namespace wscore.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("user")]
    public class LoginController : Controller
    {
        private ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _loginService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            Login login = new Login();
            login.FirstName = user.FirstName;
            login.LastName = user.LastName;
            login.Username = user.Username;
            login.Token = user.Token;

            return Ok(login);
        }

        [AllowAnonymous]
        [HttpPost("authenticateDemo")]
        public IActionResult AuthenticateDemo([FromBody]User userParam)
        {

            if ((userParam.Username != "demo") || (userParam.Password != "demo"))
                return BadRequest(new { message = "Username or password is incorrect" });

            var user = _loginService.AuthenticateDemo(userParam.Username, userParam.Password);

            Login login = new Login();
            login.FirstName = user.FirstName;
            login.LastName = user.LastName;
            login.Username = user.Username;
            login.Token = user.Token;

            return Ok(login);
        }
    }
}