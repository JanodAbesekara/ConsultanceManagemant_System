using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime AppoinmetDate { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }

        [Required]
        public string Status { get; set; } = "Available";

        [ForeignKey("DoctorDetails")]
        public int DoctorId { get; set; } 
        public  DoctorDetails DoctorDetails { get; set; }

        [ForeignKey("patient")]
        public int? patientid { get; set; }
        public patient patient { get; set; }

        // forgin keys
        public virtual AddtheSessionLink AddtheSessionLink { get; set; }



    }
}
