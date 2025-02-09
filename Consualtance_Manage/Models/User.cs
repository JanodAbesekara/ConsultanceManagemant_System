using Microsoft.AspNetCore.Identity;

namespace Consualtance_Manage.Models
{

    public enum RoleManager
    {
    Admin,
    User,
    Doctor
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public string RoleManager { get; set; }

}
}
