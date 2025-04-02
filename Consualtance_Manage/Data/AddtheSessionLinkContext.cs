using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class AddtheSessionLinkContext: DbContext
    {
        public AddtheSessionLinkContext(DbContextOptions<AddtheSessionLinkContext> options) : base(options)
        {
        }
    
        public DbSet<AddtheSessionLink> AddtheSessionLink { get; set; }
    }
}
