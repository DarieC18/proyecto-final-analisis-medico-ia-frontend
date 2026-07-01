using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class AiAnalysesRepository : BaseRepository<AiAnalysis>
    {
            public AiAnalysesRepository(MedAnalyzerContextDb context) : base(context)
            {
            
            }
    }
}
