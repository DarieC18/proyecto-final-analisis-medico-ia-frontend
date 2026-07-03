using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class AiAnalysisConfiguration : IEntityTypeConfiguration<AiAnalysis>
    {
        public void Configure(EntityTypeBuilder<AiAnalysis> builder)
        {
            builder.ToTable("AiAnalyses");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.AnalysisType).IsRequired().HasMaxLength(100);
            builder.Property(a => a.PromptUsed).IsRequired();
            builder.Property(a => a.AiResponse).IsRequired();
            builder.Property(a => a.ModelUsed).IsRequired().HasMaxLength(100);
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.IsReviewed).IsRequired();
            builder.Property(a => a.Status).IsRequired();

            builder.HasOne<Patient>(a => a.Patient)
                .WithMany(p => p.AiAnalyses)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Appointment>(a => a.Appointment)
                .WithMany(a => a.AiAnalyses)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
