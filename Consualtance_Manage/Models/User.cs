using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public string ? RoleManager { get; set; }

        public string refreashToken { get; set; } = string.Empty;
        public DateTime createdToken { get; set; } 

        public DateTime TokenExpires { get; set; }


        // one to one relationship
        public virtual Patient patient { get; set; }

        // one to one relationship
        public virtual DoctorDetails DoctorDetails { get; set; }

        // one to many relationship
        public virtual ICollection<KnowdgleBase> KnowdgleBase { get; set; }

        // one to many relationship
        public virtual ICollection<AddtheSessionLink> SessionLinks { get; set; } 


    }
}
