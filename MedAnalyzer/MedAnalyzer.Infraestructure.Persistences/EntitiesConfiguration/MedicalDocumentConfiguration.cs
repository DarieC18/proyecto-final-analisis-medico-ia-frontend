using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedAnalyzer.Infraestructure.Persistences.EntitiesConfiguration
{
    public class MedicalDocumentConfiguration : IEntityTypeConfiguration<MedicalDocument>
    {
        public void Configure(EntityTypeBuilder<MedicalDocument> builder)
        {
            builder.ToTable("MedicalDocuments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName).IsRequired().HasMaxLength(300);
            builder.Property(d => d.FileType).IsRequired().HasMaxLength(50);
            builder.Property(d => d.FilePath).IsRequired();
            builder.Property(d => d.UploadedAt).IsRequired();

            builder.HasOne<Patient>(d => d.Patient)
                .WithMany(p => p.MedicalDocuments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
