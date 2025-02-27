using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class RefreashTokenContext : DbContext
    {
        public RefreashTokenContext(DbContextOptions<RefreashTokenContext> options) : base(options)
        {

        }
        public DbSet<RefreashToken> RefreashTokens { get; set; }
    }

}