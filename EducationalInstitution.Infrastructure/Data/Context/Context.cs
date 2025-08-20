using BCrypt.Net;
using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Data.Context
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<EducationalContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new ProfessorConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new QualificationConfiguration());

            // Add audit fields
            AddAuditFields(modelBuilder);

            // Seed data
            SeedData(modelBuilder);
        }

        private void AddAuditFields(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetProperty("CreatedAt") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedAt")
                        .HasDefaultValueSql("GETUTCDATE()");
                }

                if (entityType.ClrType.GetProperty("UpdatedAt") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("UpdatedAt")
                        .HasDefaultValueSql("GETUTCDATE()");
                }
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "System",
                    Email = "admin@educational.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = Domain.Enums.UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Property("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Added && entry.Property("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
