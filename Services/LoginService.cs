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
        List<Rols> Roles();
        Rols getRolById(Rols statusParam);
        Rols getRolByName(Rols statusParam);
        void CreateUser(UserRequest statusParam);
        void UpdateUser(UserRequest statusParam);
    }

    public class Unique
    {
        public Unique()
        {
            usernameUnique = true;
            emailUnique = true;
        }

        public bool usernameUnique { get; set; }
        public bool emailUnique { get; set; }
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
                Expires = DateTime.UtcNow.AddMinutes(180),
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

        private User GetUserById(int? userId)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select a.UserId, a.UserName, a.FirstName, a.LastName, a.Email, a.Description, a.Code, c.Name from User a join UserGroup b on a.UserId = b.UserId join `Group` c on b.GroupId = c.GroupId where  a.UserId='" + userId + "'", conn);

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
                        user.accessCode = int.Parse(reader["Code"].ToString());
                    }
                }
            }
            return user;
        }
        //function used when user get updated
        private User GetUserByIdIncludePassword(int? userId)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select a.UserId, a.UserName, a.FirstName, a.LastName, a.Email, a.Description, a.Password, a.Key, a.Code, c.Name from User a join UserGroup b on a.UserId = b.UserId join `Group` c on b.GroupId = c.GroupId where  a.UserId='" + userId + "'", conn);

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
                        user.Password = reader["Password"].ToString();
                        user.Key = reader["Key"].ToString();
                        user.accessCode = int.Parse(reader["Code"].ToString());
                    }
                }
            }
            return user;
        }

        private User GetUserByUsername(string userName)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from User where UserName='" + userName + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User();
                        user.Username = reader["UserName"].ToString();
                    }
                }
            }
            return user;
        }

        private User GetUserByEmail(string email)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from User where Email='" + email + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User();
                        user.Email = reader["Email"].ToString();
                    }
                }
            }
            return user;
        }
        #region Roles
        private List<Rols> ListRoles()
        {
            List<Rols> _roles = new List<Rols>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tittan.Group", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Rols _rols = new Rols();
                        _rols.Id = int.Parse(reader["GroupId"].ToString());
                        _rols.RolName = reader["Name"].ToString();

                        _roles.Add(_rols);
                    }
                }
            }
            return _roles;
        }

        private Rols GetRolById(int? id)
        {
            Rols rol = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tittan.Group where GroupId='" + id + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rol = new Rols();
                        rol.Id = int.Parse(reader["GroupId"].ToString());
                        rol.RolName = reader["Name"].ToString();
                    }
                }
            }
            return rol;
        }

        private Rols GetRolByName(string name)
        {
            Rols rol = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tittan.Group where Name='" + name + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rol = new Rols();
                        rol.Id = int.Parse(reader["GroupId"].ToString());
                        rol.RolName = reader["Name"].ToString();
                    }
                }
            }
            return rol;
        }
        #endregion


        private static void CreateUserPassword_Hash(string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username can not be empty or contain whitespace ");
            }
            using (var hmacValue = new System.Security.Cryptography.HMACSHA512())
            {
                passwordKey = hmacValue.Key;
                passwordHash = hmacValue.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        #endregion

        public LoginService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public User Authenticate(string username, string password)
        {

            var user = GetUser(username, password);

            if (user == null)
                return null;
            //here i need to add to verify if password is equal to hash password


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
            UserReturn _getUserReturn = new UserReturn();

            if (_user != null)
            {
                _getUserReturn.Id = _user.Id;
                _getUserReturn.Username = _user.Username;
                _getUserReturn.FirstName = _user.FirstName;
                _getUserReturn.LastName = _user.LastName;
                _getUserReturn.Email = _user.Email;
                _getUserReturn.Description = _user.Description;
                _getUserReturn.Group = _user.Group;
                _getUserReturn.accessCode = _user.accessCode;
            }
            else
            {
                throw new AppExceptions("User not found");
            }
            return _getUserReturn;
        }

        public void CreateUser(UserRequest statusParam)
        {
            bool userUnique = isUserUnique(statusParam.Username, statusParam.Email);
            if (string.IsNullOrWhiteSpace(statusParam.Password))
                throw new AppExceptions("Password is required");
            Rols groupValue = new Rols();
            groupValue.RolName = statusParam.Group;
            var groupExist = getRolByName(groupValue);

            if (userUnique)
            {
                if (groupExist != null)
                {
                    User user = new User();
                    Utils.Map(user, statusParam, "Create");

                    byte[] passwordHashed, passwordKey;
                    CreateUserPassword_Hash(statusParam.Password, out passwordHashed, out passwordKey);

                    user.Password = BitConverter.ToString(passwordHashed);
                    user.Key = BitConverter.ToString(passwordKey);
                    var code = GenerateNewRandomCode();
                    bool value = CodeExist(int.Parse(code));

                    if (!value)
                    {
                        throw new AppExceptions("Code already exist");
                    }
                    user.accessCode = int.Parse(code);
                    InsertSQl(user, groupExist.Id);
                }
                else
                {
                    throw new AppExceptions("Group does not exist");
                }
            }
            else
            {
                throw new AppExceptions("User Already exist");
            }
        }

        private Code GetTerminalByCode(int code)
        {
            Code _code = null;
            //int codebase = 145236;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from User where Code= '" + code.ToString() + "' ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _code = new Code();
                        _code.code = int.Parse(reader["Code"].ToString());
                    }
                }
            }
            return _code;
        }

        private bool CodeExist(int code)
        {
            var isUnique = GetTerminalByCode(code);
            if (isUnique == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GenerateNewRandomCode()
        {
            Random generator = new Random();
            String code = generator.Next(100000, 999999).ToString("D6");

            if (code.Distinct().Count() == 1)
            {
                code = GenerateNewRandomCode();
            }
            return code;
        }

        public void UpdateUser(UserRequest statusParam)
        {
            if (statusParam == null)
            {
                throw new AppExceptions("User Id is required");
            }
            else
            {
                var user = GetUserByIdIncludePassword(statusParam.Id);
                var _updateUser = statusParam;

                if (user == null)
                {
                    throw new AppExceptions("User not found");
                }
                Unique userUnique = isUserExist(statusParam.Username, statusParam.Email);
                Rols groupValue = new Rols();
                groupValue.RolName = statusParam.Group;
                var groupExist = getRolByName(groupValue);

                if (_updateUser.Username != user.Username || _updateUser.Email != user.Email)
                {
                    if (!userUnique.usernameUnique || !userUnique.emailUnique)
                    {
                        throw new AppExceptions("Username or email already exist");
                    }
                    else
                    {
                        if (groupExist == null)
                        {
                            throw new AppExceptions("Group does not exist");
                        }
                    }
                }
                Utils.Map(user, statusParam, "Update");
                string requestCode = _updateUser.Code.ToString();
                if (user.accessCode != _updateUser.Code)
                {
                    if (requestCode.Distinct().Count() == 1)
                    {
                        throw new AppExceptions("Code can to have same digits");
                    }
                    bool value = CodeExist(int.Parse(requestCode));
                    if (!value)
                    {
                        throw new AppExceptions("Code already exist");
                    }
                    user.accessCode = int.Parse(requestCode);
                }
                user.GroupId = groupExist.Id;

                if (!string.IsNullOrWhiteSpace(statusParam.Password))
                {
                    byte[] passwordHashed, passwordKey;
                    CreateUserPassword_Hash(statusParam.Password, out passwordHashed, out passwordKey);

                    user.Password = BitConverter.ToString(passwordHashed);
                    user.Key = BitConverter.ToString(passwordKey);
                }
                UpdateUserSQL(user);
            }

        }
        #region SQl functions for updating and inserting users

        private void InsertSQl(User _insertUser, int rolId)
        {
            var _insert = _insertUser;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO tittan.User (FirstName, LastName, UserName, tittan.User.Password, tittan.User.Key, Email, Code) VALUES ('" + _insert.FirstName.ToString() + "' ,'" + _insert.LastName.ToString() + "','" + _insert.Username.ToString() + "','" + _insert.Password + "','" + _insert.Key + "','" + _insert.Email.ToString() + "','" + _insert.accessCode + "'); SELECT LAST_INSERT_ID(); ", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _insert.Id = Convert.ToInt32(reader[0]);
                    }
                }
                if (_insert.Id > 0)
                {
                    InserUserRole(_insert.Id, rolId); //this functions insert the role for the user created
                }
            }
        }

        private void InserUserRole(int userId, int rolId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tittan.UserGroup (UserId, GroupId) VALUEs(" + userId.ToString() + "," + rolId.ToString() + ");", conn);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateUserSQL(User user)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE User u1, UserGroup g2 " +
                " SET u1.FirstName = '" + user.FirstName + "' , u1.LastName = '" + user.LastName + "', u1.UserName= '" + user.Username + "' , u1.Password = '" + user.Password + "', u1.Key= '" + user.Key + "', u1.Email= '" + user.Email + "', u1.Description = '" + user.Description + "', g2.GroupId = '" + user.GroupId + "' , u1.Code = '" + user.accessCode + "' WHERE u1.UserId = '" + user.Id.ToString() + "' and g2.UserId ='" + user.Id.ToString() + "';", conn);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        //later i need to fix it because the return is not rigth should return or object of all bool
        public bool isUserUnique(string username, string email)
        {
            var isUniqueUsername = GetUserByUsername(username);
            var isUniqueEmail = GetUserByEmail(email);

            if (isUniqueUsername != null && isUniqueEmail != null)
            {
                throw new AppExceptions("Username and email already exist");
            }
            else if (isUniqueUsername != null)
            {
                throw new AppExceptions("Username already exist");
            }
            else if (isUniqueEmail != null)
            {
                throw new AppExceptions("Email already exist");
            }
            return true;

        }

        public Unique isUserExist(string username, string email)
        {
            var unique = new Unique();
            var isUniqueUsername = GetUserByUsername(username);
            var isUniqueEmail = GetUserByEmail(email);

            if (isUniqueUsername != null)
            {
                unique.usernameUnique = false;
            }
            if (isUniqueEmail != null)
            {
                unique.emailUnique = false;
            }

            return unique;
        }
        #region Roles  

        public List<Rols> Roles()
        {
            var _roles = ListRoles();
            List<Rols> __rols = new List<Rols>();
            if (_roles != null)
            {
                foreach (Rols rol in _roles)
                {
                    Rols rols = new Rols();
                    rols.Id = rol.Id;
                    rols.RolName = rol.RolName;

                    __rols.Add(rols);
                }
            }
            return __rols;
        }
        public Rols getRolById(Rols statusParam)
        {
            if (statusParam == null)
            {
                throw new AppExceptions("Rol Id is required");
            }
            var _rol = GetRolById(statusParam.Id);
            Rols _getRolReturn = new Rols();

            if (_rol != null)
            {
                _getRolReturn.Id = _rol.Id;
                _getRolReturn.RolName = _rol.RolName;
            }
            else
            {
                throw new AppExceptions("Rol not found");
            }
            return _getRolReturn;
        }

        public Rols getRolByName(Rols statusParam)
        {
            if (string.IsNullOrEmpty(statusParam.RolName))
            {
                throw new AppExceptions("Rol name is required");
            }
            var _rol = GetRolByName(statusParam.RolName);
            Rols _getRolReturn = new Rols();

            if (_rol != null)
            {
                _getRolReturn.Id = _rol.Id;
                _getRolReturn.RolName = _rol.RolName;
            }
            else
            {
                throw new AppExceptions("Rol not found");
            }
            return _getRolReturn;
        }
        #endregion



    }
}
