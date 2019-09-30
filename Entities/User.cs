
using System.ComponentModel.DataAnnotations;

namespace wscore.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }    //used for password hash
        public byte[] PasswordHash { get; set; }
        public string Key { get; set; }         //used for passwordsalt
        public string Email { get; set; }
        public string Token { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public int GroupId { get; set; }    //used to updating groupId
        public int accessCode { get; set; }       //used for access code through the terminal boot
    }

    public class UserReturn
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public int accessCode { get; set; }
    }
    public class User_Request
    {
        public int? Id { get; set; }
    }

    public class UserRequest
    {
        //[Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(".*[0-9].*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]
        public string Email { get; set; }
        [Required]
        public string Group { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Range(100000, 999999)]
       // [StringLength(6, MinimumLength = 6)]
        public int accessCode { get; set; }

    }
}
