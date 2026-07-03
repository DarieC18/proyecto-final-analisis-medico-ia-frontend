using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.DiagnosisInitial).IsRequired().HasMaxLength(500);
            builder.Property(m => m.Notes).IsRequired();
            builder.Property(m => m.Antecedentes).HasMaxLength(1000);
            builder.Property(m => m.ObservacionesConsulta).HasMaxLength(1000);
            builder.Property(m => m.CreatedAt).IsRequired();

            builder.HasOne<Appointment>(m => m.Appointment)
                .WithMany(a => a.MedicalRecords)
                .HasForeignKey(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Patient>(m => m.Patient)
            .WithMany(p => p.MedicalRecords)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
