using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Infraestructure.Identity.Seeds
{
    public class DefaultPatientUser
    {
        public async static Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                FirstName = "Juan",
                LastName = "Perez",
                Email = "patient@medanalyzer.com",
                NumberIdentification = "0000000003",
                Status = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = "PatientUser"
            };

            if (await userManager.Users.AllAsync(u => u.Email != user.Email))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if (entityUser == null)
                {
                    await userManager.CreateAsync(user, "123Pas$$word!");
                    await userManager.AddToRoleAsync(user, Role.Patient.ToString());
                }
            }
        }
    }
}
