using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class AlertsRepository : BaseRepository<Alert>
    {
        public AlertsRepository(MedAnalyzerContextDb context) : base(context)
        {

        }
    }
}
