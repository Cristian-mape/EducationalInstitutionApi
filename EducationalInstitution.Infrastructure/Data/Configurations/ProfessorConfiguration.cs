using EducationalInstitution.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Data.Configurations
{
    public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.ToTable("Professors");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(p => p.EmployeeCode)
                .IsUnique();

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(p => p.Email)
                .IsUnique();

            builder.Property(p => p.Phone)
                .HasMaxLength(20);

            builder.Property(p => p.Department)
                .HasMaxLength(200);

            builder.Property(p => p.Specialization)
                .HasMaxLength(200);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Ignore(p => p.FullName);

            // Relationships
            builder.HasMany(p => p.Courses)
                .WithOne(c => c.Professor)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
