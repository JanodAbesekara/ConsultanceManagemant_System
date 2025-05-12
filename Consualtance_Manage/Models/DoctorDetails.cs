using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.Models
{
    public enum Gendermanage
    {
        Male,
        Female,
    }

    public class DoctorDetails
    {
        [Key]
        public int Doctorid { get; set; }
        public string Specialization { get; set; }
        public string Gendermanage { get; set; }
        public int Experience { get; set; }

        // more than 5 years
        public string IsAvailable { get; set; } = "Available";
        public string Languages { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        // Foreign keys
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Ratings> Ratings { get; set; }

        // Convert  (string) to an array in 
        [NotMapped]
        public string SpecializationArray { get; set; }
        //{
        //    get => Specialization?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        //    set => Specialization = string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)));
        //}

        [NotMapped]
        public string LanguagesArray { get; set; }
       
        //{
        //    get => Languages?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        //    set => Languages = string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)));
        //}
    }
}
