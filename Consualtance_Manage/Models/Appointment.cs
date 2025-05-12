using Org.BouncyCastle.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string StartTime { get; set; }  

        [Required]
        public string EndTime { get; set; }  

        [Required]
        public string Status { get; set; } = "Available";

        [ForeignKey("DoctorDetails")]
        public int DoctorId { get; set; }

        public virtual DoctorDetails DoctorDetails { get; set; }  

        [ForeignKey("Patient")]
        public int? PatientId { get; set; } 

        public virtual Patient patient { get; set; } 

        // Foreign key for session link
        public virtual AddtheSessionLink AddtheSessionLink { get; set; }
    }
}
