using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Context
{
    public class PatientContext:DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options)
        {

        }
        public DbSet<patient> Patients { get; set; }
    }
}
