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
    public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
    {
        public void Configure(EntityTypeBuilder<Qualification> builder)
        {
            builder.ToTable("Qualifications");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Id)
                .ValueGeneratedOnAdd();

            builder.Property(q => q.Grade)
                .IsRequired()
                .HasColumnType("decimal(3,2)");

            builder.Property(q => q.Comments)
                .HasMaxLength(500);

            builder.Ignore(q => q.IsPassing);

            // Unique constraint for student-course combination
            builder.HasIndex(q => new { q.StudentId, q.CourseId })
                .IsUnique();

            // Relationships
            builder.HasOne(q => q.Student)
                .WithMany(s => s.Qualifications)
                .HasForeignKey(q => q.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Course)
                .WithMany(c => c.Qualifications)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
