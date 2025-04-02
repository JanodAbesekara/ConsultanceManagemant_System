using Microsoft.EntityFrameworkCore;
using Consualtance_Manage.Models;

namespace Consualtance_Manage.Context
{
    public class AppointmentContext:DbContext
    {
        public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options)
        {
        }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
