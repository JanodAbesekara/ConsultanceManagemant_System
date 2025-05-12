using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }


        public string Reports { get; set; } 

        [Required]
        public string Gender { get; set; }

        
        public string Languages { get; set; } 

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Ratings> Ratings { get; set; }

        //[NotMapped]
        //public string[] ReportsArray
        //{
        //    get => Reports?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        //    set => Reports = value != null ? string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s))) : "";
        //}

        //// Convert Languages string to array and vice versa
        //[NotMapped]
        //public string[] LanguagesArray
        //{
        //    get => Languages?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        //    set => Languages = value != null ? string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s))) : "";
        //}

    }
}
