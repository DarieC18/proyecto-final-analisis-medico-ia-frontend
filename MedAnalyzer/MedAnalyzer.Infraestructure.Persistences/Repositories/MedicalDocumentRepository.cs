using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class MedicalDocumentRepository : BaseRepository<MedicalDocument>, IMedicalDocumentRepository
    {
        public MedicalDocumentRepository(MedAnalyzerContextDb context) : base(context)
        {
        }
    }
}
