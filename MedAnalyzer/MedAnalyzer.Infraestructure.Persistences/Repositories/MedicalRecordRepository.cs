using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class MedicalRecordRepository : BaseRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(MedAnalyzerContextDb context) : base(context)
        {
        }
    }
}
