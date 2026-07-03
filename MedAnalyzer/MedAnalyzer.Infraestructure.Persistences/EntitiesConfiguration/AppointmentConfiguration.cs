using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.DoctorId).IsRequired().HasMaxLength(450);
            builder.Property(a => a.Status).IsRequired().HasMaxLength(30);
            builder.Property(a => a.Reason).IsRequired().HasMaxLength(500);
            builder.Property(a => a.Notes).HasMaxLength(2000);
            builder.Property(a => a.CreatedAt).IsRequired();

            builder.HasOne<Patient>(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
