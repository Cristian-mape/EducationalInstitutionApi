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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.StudentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.StudentCode)
                .IsUnique();

            builder.Property(s => s.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(s => s.Email)
                .IsUnique();

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.Ignore(s => s.FullName);

            // Relationships
            builder.HasMany(s => s.Qualifications)
                .WithOne(q => q.Student)
                .HasForeignKey(q => q.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
