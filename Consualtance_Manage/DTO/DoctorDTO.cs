using System.ComponentModel.DataAnnotations.Schema;

namespace Consualtance_Manage.DTO
{
    public class DoctorDTO
    {
        public required int Doctorid { get; set; }
        public required string Specialization { get; set; }
        public required string Gendermanage { get; set; }
        public required int Experience { get; set; }
        public required string IsAvailable { get; set; }
        public required string Languages { get; set; }
        public required int UserId { get; set; }
    }
}
