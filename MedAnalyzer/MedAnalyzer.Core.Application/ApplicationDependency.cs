using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace MedAnalyzer.Core.Application
{
    public static class ApplicationDependency
    {
        public static void AddApplicationLayer(this IServiceCollection service)
        {
            service.AddAutoMapper(cfg => cfg.AddMaps(typeof(ApplicationDependency).Assembly));

            #region Services IOC
            service.AddTransient(typeof(IBaseServices<,>), typeof(BaseServices<,>));

            service.AddScoped<IDashboardService<DoctorDashboardDto>, DoctorDashboardService>();
            service.AddScoped<IAlertService, AlertService>();
            service.AddScoped<IAiAnalisysServices, AiAnalisysServices>();
            service.AddScoped<IAppointmentService, AppointMentService>();
            service.AddScoped<IPatientService, PatientService>();
            service.AddScoped<IMedicalRecordService, MedicalRecordService>();
            service.AddScoped<ISymptomService, SymptomService>();
            service.AddScoped<IVitalSignService, VitalSignService>();
            service.AddScoped<IMedicalDocumentService, MedicalDocumentService>();
            service.AddScoped<IRecommendationService, RecommendationService>();

            service.AddScoped<IReportService, ReportService>();
            service.AddScoped<IAuditLogService, AuditLogService>();

            #endregion
        }
    }
}