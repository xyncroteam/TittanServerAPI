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
using newapi.Helpers;

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
        public IActionResult GetUser([FromBody]User_Request statusParam)
        {
            try
            {
                if (statusParam == null)
                {
                    throw new AppExceptions("Data invalid ");
                }
                var _userReturn = _loginService.getUserById(statusParam.Id);
                return Ok(_userReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserRequest statusParam)
        {
            try
            {
                if (statusParam.accessCode == 0 )
                {
                    ModelState.Remove("accessCode");
                }

                if (ModelState.IsValid)
                {
                    _loginService.CreateUser(statusParam);
                    return Ok("User register successfully");
                }
                else
                {
                    return BadRequest("Invalid data, please check the values before.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("updateuser")]
        public IActionResult UpdateUser([FromBody]UserRequest statusParam)
        {
            try
            {
                if (statusParam == null)
                {
                    throw new AppExceptions("Invalid data");
                }
                if (string.IsNullOrEmpty(statusParam.Password))
                {
                    ModelState.Remove("Password");
                }
                if (ModelState.IsValid)
                {
                    _loginService.UpdateUser(statusParam);
                    return Ok("User updated successfully");
                }
                else
                {
                    return BadRequest("Invalid data, please check the values before.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPost("getAllRoles")]
        public IActionResult GetAllUserRols()
        {
            var _getAllRoles = _loginService.Roles();
            return Ok(_getAllRoles);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("getrolbyid")]
        public IActionResult GetRolById([FromBody]Rols statusParam)
        {
            try
            {
                var _getRolById = _loginService.getRolById(statusParam);
                return Ok(_getRolById);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("getrolbyname")]
        public IActionResult GetRolByName([FromBody]Rols statusParam)
        {
            try
            {
                var _getRolByName = _loginService.getRolByName(statusParam);
                return Ok(_getRolByName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}