using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserId).IsRequired().HasMaxLength(450);
            builder.HasIndex(p => p.UserId).IsUnique();
            builder.Property(p => p.Gender).HasMaxLength(20);
            builder.Property(p => p.PhoneNumber).HasMaxLength(20);
            builder.Property(p => p.IdentificationType).HasMaxLength(50);
            builder.Property(p => p.PatientType).HasMaxLength(50);
            builder.Property(p => p.IsActive).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
        }
    }
}
