using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) :base(options) 
        {

        }
        public DbSet<User> User { get; set; }
    }
}
