
using System.ComponentModel.DataAnnotations;

namespace wscore.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
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
    }

    public class RegisterUser
    {
       [Required]
       public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
    //    [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
      //  [StringLength(100, MinimumLength = 8)]
    //    [RegularExpression(".*[0-9].*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
      //  [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")]
        public string Email { get; set; }
       [Required]
       public string Group { get; set; }

    }

   
}
