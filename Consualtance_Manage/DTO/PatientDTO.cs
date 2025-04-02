using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Consualtance_Manage.DTO
{
    public class PatientDTO
    {
        public required int PatientId { get; set; }
        public required string[] Reports { get; set; }
        public required string Gender { get; set; }
        public required string[] Languages { get; set; }
        public required int UserId { get; set; }
        public string[]? ReportsArray { get; internal set; }
        public string[]? LanguagesArray { get; internal set; }
    }
}
