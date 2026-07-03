using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Infraestructure.Identity.Seeds
{
    public class DefaultConsultationUser
    {
        public async static Task SeedAsync(UserManager<AppUser> UserManager)
        {
            AppUser user = new()
            {
                FirstName = "Juan",
                LastName = "Perez",
                Email = "consultation@medanalyzer.com",
                NumberIdentification = "0000000003",
                Status = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = "ConsultationUser"
            };

            if (await UserManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await UserManager.FindByEmailAsync(user.Email);
                if (entityUser == null)
                {
                    await UserManager.CreateAsync(user, "123Pas$$word!");
                    await UserManager.AddToRoleAsync(user, Role.ConsultationUser.ToString());
                }
            }
        }
    }
}
