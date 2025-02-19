namespace Consualtance_Manage.Models
{
    public enum Gender
    {
        Male,
        Female,
    }
    public class DoctorDetails

    {
        public int id;
        public string[] Specialization;
        public string Gender;
        public int Experience;
        public Boolean IsAvailable;


    }
}
