using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;


namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(MedAnalyzerContextDb context) : base(context)
        {

        }
    }
}
