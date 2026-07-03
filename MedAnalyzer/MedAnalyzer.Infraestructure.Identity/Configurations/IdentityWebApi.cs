using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Setting;
using MedAnalyzer.Infraestructure.Identity.Context;
using MedAnalyzer.Infraestructure.Identity.Entities;
using MedAnalyzer.Infraestructure.Identity.Seeds;
using MedAnalyzer.Infraestructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace MedAnalyzer.Infraestructure.Identity.Configurations
{
    public static class IdentityWebApi
    {
        public static void AddIdentityLayerIocForWebApi(this IServiceCollection services, IConfiguration config)
        {
            GeneralConfiguration(services, config);

            #region Configurations
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            #endregion

            #region Identity
            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 5;

                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            });

            services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12);
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"] ?? ""))
                };
                opt.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = af =>
                    {
                        af.NoResult();
                        af.Response.StatusCode = 500;
                        af.Response.ContentType = "text/plain";
                        return af.Response.WriteAsync(af.Exception.Message.ToString());
                    },
                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new { HasError = true, Error = "You are not Authorized" });
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new { HasError = true, Error = "You are not Authorized to access this resource" });
                        return c.Response.WriteAsync(result);
                    }
                };
            });
            #endregion

            #region Services
            services.AddScoped<IAccountServiceForWebApi, AccountServiceForWebApi>();
            services.AddScoped<IBaseAccountService>(sp => sp.GetRequiredService<IAccountServiceForWebApi>());
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            #endregion
        }

        public static async Task RunSeedAsync(this IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var servicesProvider = scope.ServiceProvider;

            var userManager = servicesProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaulRoles.SeedAsync(roleManager);
            await DefaultAdminUser.SeedAsync(userManager);
            await DefaultDoctorUser.SeedAsync(userManager);
            await DefaultConsultationUser.SeedAsync(userManager);
        }

        private static void GeneralConfiguration(IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("ConnectionDb");
            services.AddDbContext<IdentityContext>((sp, opt) =>
            {
                opt.EnableSensitiveDataLogging();
                opt.UseNpgsql(connectionString,
                    m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
        }
    }
}