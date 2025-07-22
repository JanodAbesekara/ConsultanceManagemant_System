namespace Consualtance_Manage.DTO
{
    public class RatingDTO
    {

        public required int RatingId { get; set; }
        public required int DoctorId { get; set; }
        public  int? PatientId { get; set; }
        public required int Rating { get; set; }
        public required string Review { get; set; }
    }

    public class FullRatings
    {
        public required int RatingId { get; set; }
        public required int DoctorId { get; set; }
        public  int? PatientId { get; set; }
        public required int Rating { get; set; }
        public required string Review { get; set; }
        public  string? patientEmail { get; set; }
        public  string? patientName { get; set; }

        public required string doctorEmail { get; set; }
        public required string doctorName { get; set; }
        
    }

    public class DoctorRatingsResponse
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorEmail { get; set; } = string.Empty;
        public double AverageRating { get; set; }
 
        public List<RatingReview> Ratings { get; set; } = new();
    }

    public class RatingReview
    {
        public int Rating { get; set; }
        public string Review { get; set; } = string.Empty;

        public required string PatientEmail { get; set; }
        public required string PatientName { get; set; }
        public required string PatientPhone { get; set; }
    }

}
