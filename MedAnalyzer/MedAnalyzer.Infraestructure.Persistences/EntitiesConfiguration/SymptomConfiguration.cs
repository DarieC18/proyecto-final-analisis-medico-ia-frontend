using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class SymptomConfiguration : IEntityTypeConfiguration<Symptom>
    {
        public void Configure(EntityTypeBuilder<Symptom> builder)
        {
            builder.ToTable("Symptoms");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Severity).IsRequired().HasMaxLength(20);
            builder.Property(s => s.StartedAt).IsRequired();
            builder.Property(s => s.Notes).HasMaxLength(500);

            builder.HasOne<Appointment>(s => s.Appointment)
                .WithMany(a => a.Symptoms)
                .HasForeignKey(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
