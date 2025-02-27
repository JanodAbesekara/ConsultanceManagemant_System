using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{

    public enum gender
    {
        Male,
        Femae,
    }
    public class patient 
    {
        [Key]
        public int patientid { get; set; }

        [Required]
        public string[] reports { get; set; }
        [Required]
        public string gender { get; set; }

        [Required]
        public string[] languages { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }  
        public  User User { get; set; }

  
        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Ratings> Ratings { get; set; }

    }
}
