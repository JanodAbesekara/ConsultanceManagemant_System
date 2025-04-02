namespace Consualtance_Manage.Models
{
    public class RefreashToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; }
    }
}
