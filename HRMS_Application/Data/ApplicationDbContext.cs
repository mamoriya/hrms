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
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }

        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }


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

            modelBuilder.Entity<EmployeeDocument>(entity =>
            {
                entity.ToTable("EmployeeDocument");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FileName)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.FileType)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.FilePath)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.FileSize)
                      .IsRequired();

                entity.Property(e => e.UploadedAt)
                      .IsRequired();

                entity.HasOne<EmployeeProfile>()
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeProfileId)
                      .IsRequired();
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.ToTable("LeaveType");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(e => e.Name)
                      .IsUnique();

                entity.Property(e => e.AnnualQuota)
                      .IsRequired();

                entity.Property(e => e.CarryForward)
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

            modelBuilder.Entity<LeaveBalance>(entity =>
            {
                entity.ToTable("LeaveBalance");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.TotalAllotted)
                      .IsRequired();

                entity.Property(e => e.Used)
                      .IsRequired();

                entity.Property(e => e.Reserved)
                      .IsRequired();

                entity.HasIndex(e => new
                {
                    e.EmployeeProfileId,
                    e.LeaveTypeId
                })
                .IsUnique();

                entity.HasOne<EmployeeProfile>()
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeProfileId)
                      .IsRequired();

                entity.HasOne<LeaveType>()
                      .WithMany()
                      .HasForeignKey(e => e.LeaveTypeId)
                      .IsRequired();
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.ToTable("LeaveRequest");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.StartDate)
                      .IsRequired();

                entity.Property(e => e.EndDate)
                      .IsRequired();

                entity.Property(e => e.Reason)
                      .HasMaxLength(500);

                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.RejectionReason)
                      .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                      .IsRequired();

                entity.HasOne<EmployeeProfile>()
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeProfileId)
                      .IsRequired();

                entity.HasOne<LeaveType>()
                      .WithMany()
                      .HasForeignKey(e => e.LeaveTypeId)
                      .IsRequired();
            });
        }
    }
}
