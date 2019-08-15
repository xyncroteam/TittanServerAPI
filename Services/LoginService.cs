using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wscore.Entities;
using wscore.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using newapi.Helpers;




namespace wscore.Services
{
    public interface ILoginService
    {
        User Authenticate(string username, string password);
        User AuthenticateDemo(string username, string password);
        List<UserReturn> Users();
        UserReturn getUserById(UserReturn statusParam);
      //  void Create(RegisterUser statusParam);
    }

    public class LoginService : ILoginService
    {
        private readonly AppSettings _appSettings;

        #region Demo

        public User AuthenticateDemo(string username, string password)
        {

            //            var user = GetUser(username, password);

            User user = new User();
            
            user.Id = 99999;
            user.Username = "Demo";
            user.FirstName = "Demo";
            user.LastName = "Demo";
            user.Email = "Demo@Demo.com";
            user.Group = "User";

            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role , user.Group),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        #endregion

        #region Private

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_appSettings.DefaultConnection);
        }

        private User GetUser(string userName, string password)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from User a join UserGroup b on a.UserId = b.UserId join `Group` c on b.GroupId = c.GroupId where UserName='" + userName + "' and Password='" + password + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User();
                        user.Id = Convert.ToInt32(reader["UserId"]);
                        user.Username = reader["UserName"].ToString();
                        user.FirstName = reader["FirstName"].ToString();
                        user.LastName = reader["LastName"].ToString();
                        user.Email = reader["Email"].ToString();
                        user.Group = reader["Name"].ToString();
                    }
                }
            }


            return user;
        }

        private User GetUserById(int? userId)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select a.UserId, a.UserName, a.FirstName, a.LastName, a.Email, a.Description, c.Name from User a join UserGroup b on a.UserId = b.UserId join `Group` c on b.GroupId = c.GroupId where  a.UserId='" + userId + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User();
                        user.Id = Convert.ToInt32(reader["UserId"]);
                        user.Username = reader["UserName"].ToString();
                        user.FirstName = reader["FirstName"].ToString();
                        user.LastName = reader["LastName"].ToString();
                        user.Email = reader["Email"].ToString();
                        user.Group = reader["Name"].ToString();
                        user.Description = reader["Description"].ToString();
                    }
                }
            }
            return user;
        }

        #endregion

        public LoginService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            
            var user =GetUser(username, password);

            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role , user.Group),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        private List<User> ListUsers()
        {
            List<User> _listUsers = new List<User>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT tittan.User.UserId, tittan.User.UserName , tittan.User.FirstName, tittan.User.lastname, tittan.Group.Name, tittan.User.Email FROM tittan.User inner JOIN tittan.UserGroup ON tittan.UserGroup.UserId = tittan.User.UserId inner join tittan.Group on tittan.Group.GroupId = tittan.UserGroup.GroupId ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User _user = new User();
                        _user.Id = int.Parse(reader["UserId"].ToString());
                        _user.Username = reader["Username"].ToString();
                        _user.FirstName = reader["FirstName"].ToString();
                        _user.LastName = reader["LastName"].ToString();
                        _user.Email = reader["Email"].ToString();
                        _user.Group = reader["Name"].ToString();

                        _listUsers.Add(_user);
                    }
                }
            }
            return _listUsers;
        }

        public List<UserReturn> Users()
        {
            var _users = ListUsers();
            List<UserReturn> __userListReturn = new List<UserReturn>();
            if (_users != null)
            {
                foreach (User user in _users)
                {
                    UserReturn ur = new UserReturn();
                    ur.Id = user.Id;
                    ur.Username = user.Username;
                    ur.FirstName = user.FirstName;
                    ur.LastName = user.LastName;
                    ur.Email = user.Email;
                    ur.Group = user.Group.ToString();

                    __userListReturn.Add(ur);
                }
            }
            return __userListReturn;
        }

        public UserReturn getUserById(UserReturn statusParam)
        {
            if (statusParam == null)
            {
                throw new AppExceptions("User Id is required");
            }

            var _user = GetUserById(statusParam.Id);

            if (_user == null)
            {
                throw new AppExceptions("User not found");
            }
            UserReturn _getUserReturn = new UserReturn();

            if(_getUserReturn != null)
            {
                _getUserReturn.Id = _user.Id;
                _getUserReturn.Username = _user.Username;
                _getUserReturn.FirstName = _user.FirstName;
                _getUserReturn.LastName = _user.LastName;
                _getUserReturn.Email = _user.Email;
                _getUserReturn.Description = _user.Description;
                _getUserReturn.Group = _user.Group;
            }
            else
            {
                throw new AppExceptions("User not found");
            }

            return _getUserReturn;
        }

       


    }
}
