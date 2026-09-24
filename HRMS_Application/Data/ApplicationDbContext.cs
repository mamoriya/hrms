using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Role)
                      .IsRequired()
                      .HasMaxLength(20);
            });


            modelBuilder.Entity<EmployeeProfile>(entity =>
            {
                entity.ToTable("EmployeeProfile");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                      .HasMaxLength(20);

                entity.Property(e => e.Address)
                      .HasMaxLength(500);

                entity.Property(e => e.JobTitle)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.JoiningDate)
                      .IsRequired();

                entity.HasOne(e => e.User)
       .WithOne()
       .HasForeignKey<EmployeeProfile>(e => e.UserId)
       .IsRequired();

            });
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                      .IsUnique();
            });
        }
    }
}
