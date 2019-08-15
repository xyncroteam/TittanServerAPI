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

        [Authorize(Roles = "Admin,User")]
        [HttpPost("listUsers")]
        public IActionResult ListUsers()
        {
            var _usersReturn = _loginService.Users(); ;
            return Ok(_usersReturn);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("getUser")]
        public IActionResult GetUser([FromBody]UserReturn statusParam)
        {
            try
            {
                var _userReturn = _loginService.getUserById(statusParam);
                return Ok(_userReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       // [Authorize(Roles = "Admin, User")]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterUser statusParam)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                   // var result =  _userManager.CreateAsync(statusParam, statusParam.Password);

                    //if (result.IsCompleted)
                    //{
                        // _logger.LogInformation("User created a new account with password.");
                        return Ok("User register successfully");
                    //}
                    //else
                    //{
                    //    //AddErrors(result);
                    //    return BadRequest("Invalid data");
                    //}                      
                    
                //}
                //else
                //{
                //    return BadRequest("Invalid data");
                //}
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //}


    }
}