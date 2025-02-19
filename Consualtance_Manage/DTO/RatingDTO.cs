namespace Consualtance_Manage.DTO
{
    public class RatingDTO
    {

        public required int DoctorId { get; set; }
        public required int PatientId { get; set; }
        public required int Rating { get; set; }
        public required string Review { get; set; }
    }
}
