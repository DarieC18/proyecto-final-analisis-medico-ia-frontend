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
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Gender).IsRequired().HasMaxLength(20);
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(p => p.IdentificationNumber).IsRequired().HasMaxLength(50);
            builder.HasIndex(p => p.IdentificationNumber).IsUnique();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.IdentificationType).IsRequired().HasMaxLength(50);
            builder.Property(p => p.PatientType).IsRequired().HasMaxLength(50);
            builder.Property(p => p.IsActive).IsRequired();
        }
    }
}
