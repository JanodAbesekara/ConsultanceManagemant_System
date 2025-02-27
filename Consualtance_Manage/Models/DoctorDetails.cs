using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public enum Gender
    {
        Male,
        Female,
    }
    public class DoctorDetails

    {
        [Key]
        public int Doctorid { get; set; }
        public string[] Specialization { get; set; }
        public string Gender { get; set; }
        public int Experience { get; set; }

        // more than 5 years
        public Boolean IsAvailable { get; set; }

        public string[] Languages { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; }  
        public  User User { get; set; }

       
        // forgin keys
        public ICollection<Appointment> Appointments { get; set; }

 
        public ICollection<Ratings> Ratings { get; set; }


    }
}
