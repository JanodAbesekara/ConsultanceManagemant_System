using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class AddtheSessionLink
    {
        [Key]
        public int sessionLinkId { get; set; }
        [Required]
        public string sessionLink { get; set; }

        public string? Message { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User user { get; set; }

    }
}
