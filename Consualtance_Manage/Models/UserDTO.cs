namespace Consualtance_Manage.Models
{
    public class UserDTO
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }

        public required string RoleManager { get; set; }
    }

    public class LoginDTO
    {
        public required string Email {get;set;}
        public required string Password { get; set; }

    }

    public class Getallusers
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Phone { get; set; }

    }

}
