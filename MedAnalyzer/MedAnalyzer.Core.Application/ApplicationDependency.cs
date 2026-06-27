using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MedAnalyzer.Core.Application
{
    public static class ApplicationDependency
    {
        public static void AddApplicationLayer(this IServiceCollection service)
        {
            service.AddAutoMapper(cfg => { }, typeof(ApplicationDependency));

            #region Services IOC
            service.AddTransient(typeof(IBaseServices<,>), typeof(BaseServices<,>));
            service.AddScoped<IAuditLogService, AuditLogService>();



            #endregion

        }
    }
}
