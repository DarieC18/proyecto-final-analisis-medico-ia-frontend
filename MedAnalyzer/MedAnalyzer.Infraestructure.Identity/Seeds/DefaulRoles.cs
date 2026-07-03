using MedAnalyzer.Core.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace MedAnalyzer.Infraestructure.Identity.Seeds
{
    public static class DefaulRoles
    {
        public async static Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Role.Administrator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Doctor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Nurse.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.ConsultationUser.ToString()));

        }
    }
}
