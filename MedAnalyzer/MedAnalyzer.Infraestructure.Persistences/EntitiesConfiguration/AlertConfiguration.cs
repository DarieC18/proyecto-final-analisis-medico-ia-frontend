using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("Alerts");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(300);
            builder.Property(a => a.Description).IsRequired();
            builder.Property(a => a.Severity).IsRequired().HasMaxLength(20);
            builder.Property(a => a.IsResolved).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();

            builder.HasOne<Patient>(a => a.Patient)
                .WithMany(p => p.Alerts)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
