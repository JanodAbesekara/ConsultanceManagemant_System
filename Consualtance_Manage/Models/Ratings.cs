namespace Consualtance_Manage.Models
{
    public class Ratings
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
    }
}
