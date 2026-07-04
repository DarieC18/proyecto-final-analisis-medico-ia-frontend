using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class VitalSignService : BaseServices<VitalSign, VitalSignDto>, IVitalSignService
    {
        private readonly IBaseRepository<VitalSign> _repository;
        private readonly IBaseRepository<Appointment> _appointmentRepository;
        private readonly IAlertService _alertService;
        private readonly IMapper _mapper;

        public VitalSignService(
            IMapper mapper,
            IBaseRepository<VitalSign> repository,
            IBaseRepository<Appointment> appointmentRepository,
            IAlertService alertService)
            : base(mapper, repository)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _alertService = alertService;
            _mapper = mapper;
        }

        public async Task<List<VitalSignDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<VitalSignDto>>(
                all.Where(v => v.AppointmentId == appointmentId).ToList());
        }

        public override async Task<VitalSignDto?> SaveDtoAsync(VitalSignDto dto)
        {
            Validate(dto);
            return await base.SaveDtoAsync(dto);
        }

        public override async Task<VitalSignDto?> UpdateDtoAsync(VitalSignDto dto, int id)
        {
            Validate(dto);
            return await base.UpdateDtoAsync(dto, id);
        }

        private static void Validate(VitalSignDto dto)
        {
            if (dto.AppointmentId == 0)
                throw new DomainValidationException("La cita asociada es requerida.");
            if (dto.Temperature < 30m || dto.Temperature > 45m)
                throw new DomainValidationException("La temperatura debe estar entre 30 y 45 °C.");
            if (dto.HeartRate <= 0)
                throw new DomainValidationException("La frecuencia cardíaca debe ser un valor positivo.");
            if (dto.SystolicPressure <= 0 || dto.DiastolicPressure <= 0)
                throw new DomainValidationException("Los valores de presión arterial deben ser positivos.");
            if (dto.OxygenSaturation < 0 || dto.OxygenSaturation > 100)
                throw new DomainValidationException("La saturación de oxígeno debe estar entre 0 y 100%.");
        }

        public async Task<VitalSignDto?> RegisterWithAlerts(VitalSignDto dto)
        {
            var saved = await SaveDtoAsync(dto);
            if (saved == null) return null;

            var appointment = await _appointmentRepository.GetEntityByIdAsync(dto.AppointmentId);
            if (appointment == null) return saved;

            var alerts = BuildAlerts(dto, appointment.PatientId, dto.AppointmentId);
            foreach (var alert in alerts)
                await _alertService.SaveDtoAsync(alert);

            return saved;
        }

        private static List<AlertDto> BuildAlerts(VitalSignDto v, int patientId, int appointmentId)
        {
            var alerts = new List<AlertDto>();

            if (v.Temperature < 35m)
                alerts.Add(Alert(patientId, appointmentId, "Hipotermia",
                    $"Temperatura {v.Temperature}°C por debajo del umbral crítico (35°C).", "Alta"));
            else if (v.Temperature >= 39m)
                alerts.Add(Alert(patientId, appointmentId, "Fiebre alta",
                    $"Temperatura {v.Temperature}°C indica fiebre alta.", "Alta"));
            else if (v.Temperature >= 38m)
                alerts.Add(Alert(patientId, appointmentId, "Fiebre leve",
                    $"Temperatura {v.Temperature}°C indica fiebre leve.", "Media"));

            if (v.HeartRate < 50)
                alerts.Add(Alert(patientId, appointmentId, "Bradicardia severa",
                    $"Frecuencia cardíaca {v.HeartRate} lpm por debajo del umbral crítico.", "Alta"));
            else if (v.HeartRate < 60)
                alerts.Add(Alert(patientId, appointmentId, "Bradicardia",
                    $"Frecuencia cardíaca {v.HeartRate} lpm por debajo del rango normal.", "Media"));
            else if (v.HeartRate > 120)
                alerts.Add(Alert(patientId, appointmentId, "Taquicardia severa",
                    $"Frecuencia cardíaca {v.HeartRate} lpm por encima del umbral crítico.", "Alta"));
            else if (v.HeartRate > 100)
                alerts.Add(Alert(patientId, appointmentId, "Taquicardia",
                    $"Frecuencia cardíaca {v.HeartRate} lpm por encima del rango normal.", "Media"));

            if (v.SystolicPressure < 90)
                alerts.Add(Alert(patientId, appointmentId, "Hipotensión",
                    $"Presión sistólica {v.SystolicPressure} mmHg por debajo del rango normal.", "Alta"));
            else if (v.SystolicPressure >= 140)
                alerts.Add(Alert(patientId, appointmentId, "Hipertensión sistólica",
                    $"Presión sistólica {v.SystolicPressure} mmHg por encima del rango normal.", "Alta"));

            if (v.DiastolicPressure < 60)
                alerts.Add(Alert(patientId, appointmentId, "Presión diastólica baja",
                    $"Presión diastólica {v.DiastolicPressure} mmHg por debajo del rango normal.", "Media"));
            else if (v.DiastolicPressure >= 90)
                alerts.Add(Alert(patientId, appointmentId, "Presión diastólica alta",
                    $"Presión diastólica {v.DiastolicPressure} mmHg por encima del rango normal.", "Media"));

            if (v.OxygenSaturation < 90m)
                alerts.Add(Alert(patientId, appointmentId, "Hipoxia severa",
                    $"Saturación de oxígeno {v.OxygenSaturation}% en nivel crítico.", "Alta"));
            else if (v.OxygenSaturation < 95m)
                alerts.Add(Alert(patientId, appointmentId, "Saturación de oxígeno baja",
                    $"Saturación de oxígeno {v.OxygenSaturation}% por debajo del rango normal.", "Media"));

            if (v.Glucose.HasValue)
            {
                if (v.Glucose.Value < 70m)
                    alerts.Add(Alert(patientId, appointmentId, "Hipoglucemia",
                        $"Glucosa {v.Glucose}mg/dL por debajo del rango normal.", "Alta"));
                else if (v.Glucose.Value > 200m)
                    alerts.Add(Alert(patientId, appointmentId, "Hiperglucemia",
                        $"Glucosa {v.Glucose}mg/dL por encima del rango normal.", "Alta"));
            }

            return alerts;
        }

        private static AlertDto Alert(int patientId, int appointmentId, string title, string description, string severity)
            => new()
            {
                Id = 0,
                PatientId = patientId,
                AppointmentId = appointmentId,
                Title = title,
                Description = description,
                Severity = severity,
                IsResolved = false
            };
    }
}
