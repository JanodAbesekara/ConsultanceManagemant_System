using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class RatingContext:DbContext
    {
        public RatingContext(DbContextOptions<RatingContext> options): base(options)
        {

        }

        public DbSet<Ratings> ratins { get; set; }
    }

}
