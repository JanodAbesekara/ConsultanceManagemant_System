namespace Consualtance_Manage.Models
{

    public enum gender
    {
        Male,
        Femae,
    }
    public class patient
    {
        public int id { get; set; }
        public string[] Reports { get; set; }
        public string gender { get; set; }

    }
}
