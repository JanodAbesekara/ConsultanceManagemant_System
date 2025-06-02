using Consualtance_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Consualtance_Manage.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorDetails> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Ratings> Ratings { get; set; }
        public DbSet<AddtheSessionLink> AddtheSessionLinks { get; set; }
        public DbSet<KnowdgleBase> KnowdgleBase { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DoctorDetails>().ToTable("DoctorDetails");
            modelBuilder.Entity<Patient>().ToTable("patient");

            // One-to-One Relationship: User ↔ Patient
            modelBuilder.Entity<User>()
                .HasOne(u => u.patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One Relationship: User ↔ DoctorDetails
            modelBuilder.Entity<User>()
                .HasOne(u => u.DoctorDetails)
                .WithOne(d => d.User)
                .HasForeignKey<DoctorDetails>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many Relationship: User ↔ AddtheSessionLink
            modelBuilder.Entity<User>()
                .HasMany(u => u.SessionLinks)
                .WithOne(s => s.user)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many Relationship: User ↔ KnowdgleBase
            modelBuilder.Entity<User>()
                .HasMany(u => u.KnowdgleBase)
                .WithOne(kb => kb.user)
                .HasForeignKey(kb => kb.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many Relationship: DoctorDetails ↔ Appointments
            modelBuilder.Entity<DoctorDetails>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.DoctorDetails)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many Relationship: Patient ↔ Appointments
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // One-to-Many Relationship: DoctorDetails ↔ Ratings
            modelBuilder.Entity<DoctorDetails>()
                .HasMany(d => d.Ratings)
                .WithOne(r => r.DoctorDetails)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            // One-to-Many Relationship: Patient ↔ Ratings
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Ratings)
                .WithOne(r => r.patient)
                .HasForeignKey(r => r.PatientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


            // One-to-One Relationship: Appointment ↔ AddtheSessionLink
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AddtheSessionLink)
                .WithOne(s => s.Appointment)
                .HasForeignKey<AddtheSessionLink>(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}
