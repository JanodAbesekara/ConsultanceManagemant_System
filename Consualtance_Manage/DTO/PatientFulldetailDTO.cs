namespace Consualtance_Manage.DTO
{
    public class PatientFulldetailDTO
    {
        public required int PatientId { get; set; }
        public required string Reports { get; set; }
        public required string Gender { get; set; }
        public required string Languages { get; set; }
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
    }
}
