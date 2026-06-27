using MedAnalyzer.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MedAnalyzer.Infraestructure.Persistences.Context
{
    public class MedAnalyzerContextDb : DbContext
    {
        public MedAnalyzerContextDb(DbContextOptions<MedAnalyzerContextDb> options) : base(options)
        { }


        public DbSet<AiAnalysis> AiAnalyses { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<MedicalDocument> MedicalDocuments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

    }
}
    



