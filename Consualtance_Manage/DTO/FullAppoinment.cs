namespace Consualtance_Manage.DTO
{
    public class FullAppoinment
    {
        public required int AppointmentId { get; set; }
        public required DateTime AppointmentDate { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public required string Status { get; set; }
        public required string Specialization { get; set; }
        public required string Gendermanage { get; set; }
        public required int Experience { get; set; }
        public required string IsAvailable { get; set; }
        public required string Languages { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
