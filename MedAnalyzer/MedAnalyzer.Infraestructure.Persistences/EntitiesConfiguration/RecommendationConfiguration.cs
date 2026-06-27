using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
    {
        public void Configure(EntityTypeBuilder<Recommendation> builder)
        {
            builder.ToTable("Recommendations");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(300);
            builder.Property(r => r.Description).IsRequired();
            builder.Property(r => r.RiskLevel).IsRequired().HasMaxLength(20);
            builder.Property(r => r.CreatedAt).IsRequired();

            builder.HasOne<AiAnalysis>(r => r.AiAnalysis)
                .WithMany(a => a.AiAnalysisResults)
                .HasForeignKey(r => r.AiAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
