using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Models
{
    public class RefreashTokenContext : DbContext
    {
        public RefreashTokenContext(DbContextOptions<RefreashTokenContext> options) : base(options)
        {

        }
        public DbSet<RefreashToken> RefreashTokens { get; set; }
    }

}