using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class Ratings
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public string Review { get; set; }

        [ForeignKey("DoctorDetails")]
        public int DoctorId { get; set; }
        public DoctorDetails DoctorDetails { get; set; }

        [ForeignKey("patient")]
        public int? PatientId { get; set; }
        public Patient patient { get; set; }
    }
}
