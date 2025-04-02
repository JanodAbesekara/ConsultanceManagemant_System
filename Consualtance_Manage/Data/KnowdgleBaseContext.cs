using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class KnowdgleBaseContext: DbContext
    {
        public KnowdgleBaseContext(DbContextOptions<KnowdgleBaseContext> options) : base(options)
        {
        }
        public DbSet<KnowdgleBase> knowdglebase { get; set; }
    }
    


    
}
