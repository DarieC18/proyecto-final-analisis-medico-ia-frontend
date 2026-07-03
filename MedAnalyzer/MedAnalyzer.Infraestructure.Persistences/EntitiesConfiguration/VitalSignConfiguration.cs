using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class VitalSignConfiguration : IEntityTypeConfiguration<VitalSign>
    {
        public void Configure(EntityTypeBuilder<VitalSign> builder)
        {
            builder.ToTable("VitalSigns");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Temperature).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(v => v.HeartRate).IsRequired();
            builder.Property(v => v.SystolicPressure).IsRequired();
            builder.Property(v => v.OxygenSaturation).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(v => v.Glucose).HasColumnType("decimal(6,2)");

            builder.HasOne<Appointment>(v => v.Appointment)
                .WithMany(a => a.VitalSigns)
                .HasForeignKey(v => v.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
