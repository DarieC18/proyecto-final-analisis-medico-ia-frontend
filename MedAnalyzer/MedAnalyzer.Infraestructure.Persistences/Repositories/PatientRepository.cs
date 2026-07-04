using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Infraestructure.Persistences.Context;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class PatientRepository : BaseRepository<Patient>
    {
        public PatientRepository(MedAnalyzerContextDb context) : base(context)
        {
        }
    }
}
