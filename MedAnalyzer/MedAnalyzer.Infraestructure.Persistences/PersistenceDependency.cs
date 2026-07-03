using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;
using MedAnalyzer.Infraestructure.Persistences.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MedAnalyzer.Infraestructure.Persistences
{
    public static class PersistenceDependency
    {
        public static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            var connectionString = config.GetConnectionString("ConnectionDb");
            services.AddDbContext<MedAnalyzerContextDb>(opt =>
                opt.UseNpgsql(connectionString,
                m => m.MigrationsAssembly(typeof(MedAnalyzerContextDb).Assembly.FullName)),
                ServiceLifetime.Scoped);

            #endregion
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository< >));

            services.AddScoped<IAuditLogRepository, AuditLogRepository>();


        }
    }
}
