using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class DoctorContext: DbContext
    {
        public DoctorContext(DbContextOptions<DoctorContext> options) : base(options)
        {
        }
        public DbSet<DoctorDetails> Doctors { get; set; }
    }
     
}
