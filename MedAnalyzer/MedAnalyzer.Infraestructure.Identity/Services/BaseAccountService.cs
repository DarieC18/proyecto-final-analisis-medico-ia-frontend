using MedAnalyzer.Core.Application.Dto.Email;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Core.Domain.Setting;
using MedAnalyzer.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace MedAnalyzer.Infraestructure.Identity.Services
{
    public abstract class BaseAccountService : IBaseAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;

        protected BaseAccountService(UserManager<AppUser> userManager, IEmailService emailService, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _emailService = emailService;
            _appSettings = appSettings.Value;
        }

        public virtual async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, string? currentUserId = null)
        {
            RegisterResponseDto response = new()
            {
                Id = "",
                Name = "",
                LastName = "",
                Email = "",
                UserName = "",
                NumberIdentificacion = "",
                HasError = false,
                Errors = []
            };

            var userWithSameUserName = await _userManager.FindByNameAsync(dto.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Errors.Add($"El nombre de usuario: {dto.UserName} ya está en uso.");
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Errors.Add($"El correo electrónico: {dto.Email} ya está registrado.");
                return response;
            }

            var userWithSameIdentification = await _userManager.Users.AnyAsync(i => i.NumberIdentification == dto.NumberIdentification);
            if (userWithSameIdentification)
            {
                response.HasError = true;
                response.Errors.Add($"El número de cédula {dto.NumberIdentification} ya está registrado.");
                return response;
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                response.HasError = true;
                response.Errors.Add("Las contraseñas no coinciden.");
                return response;
            }

            var restrictedRoles = new[] { Role.Doctor.ToString(), Role.Nurse.ToString(), Role.Administrator.ToString() };

            if (string.IsNullOrWhiteSpace(currentUserId) && !string.IsNullOrWhiteSpace(dto.Role))
            {
                if (restrictedRoles.Contains(dto.Role, StringComparer.OrdinalIgnoreCase))
                {
                    response.HasError = true;
                    response.Errors.Add($"El rol '{dto.Role}' solo puede ser asignado por un administrador.");
                    return response;
                }
            }

            bool isAdmin = false;
            string role = Role.ConsultationUser.ToString();
            if (!string.IsNullOrWhiteSpace(currentUserId))
            {
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, Role.Administrator.ToString()))
                {
                    isAdmin = true;
                    role = string.IsNullOrWhiteSpace(dto.Role) ? Role.ConsultationUser.ToString() : dto.Role;
                }
            }

            AppUser user = new()
            {
                FirstName = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                NumberIdentification = dto.NumberIdentification,
                EmailConfirmed = isAdmin,
                Status = isAdmin
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);

                if (!isAdmin)
                {
                    var verificationUri = await GetVerificationEmailUri(user);
                    await _emailService.SendEmailAsync(new EmailRequestDto()
                    {
                        To = dto.Email,
                        HtmlBody = $"Por favor confirma tu cuenta haciendo clic aquí: <a href='{verificationUri}'>Confirmar email</a>",
                        Subject = "Confirmación de registro"
                    });
                }

                response.Id = user.Id;
                response.Name = user.FirstName;
                response.LastName = user.LastName;
                response.Email = user.Email ?? "";
                response.UserName = user.UserName ?? "";

                return response;
            }
            else
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }
        }

        public virtual async Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No existe una cuenta registrada con este nombre de usuario: {request.UserName}");
                return response;
            }

            user.Status = false;
            await _userManager.UpdateAsync(user);

            var resetUri = await GetResetPasswordUri(user);
            await _emailService.SendEmailAsync(new EmailRequestDto()
            {
                To = user.Email,
                HtmlBody = $"Por favor restablece tu contraseña haciendo clic aquí: <a href='{resetUri}'>Restablecer pass</a>",
                Subject = "Restablecer contraseña"
            });

            return response;
        }

        public virtual async Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("No existe una cuenta registrada con este usuario.");
                return response;
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }

            user.Status = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return response;
        }

        public virtual async Task<UserResponseDto> DeleteAsync(string id)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("No existe una cuenta registrada con este usuario.");
                return response;
            }

            await _userManager.DeleteAsync(user);

            return response;
        }

        public virtual async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Name = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                NumberIdentification = user.NumberIdentification,
                isVerified = user.EmailConfirmed,
                Role = rolesList.FirstOrDefault() ?? "",
                Status = user.Status
            };
        }

        public virtual async Task<UserDto?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Name = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                NumberIdentification = user.NumberIdentification,
                isVerified = user.EmailConfirmed,
                Role = rolesList.FirstOrDefault() ?? "",
                Status = user.Status
            };
        }

        public virtual async Task<UserDto?> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            return new UserDto()
            {
                Id = user.Id,
                Name = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                NumberIdentification= user.NumberIdentification,
                isVerified = user.EmailConfirmed,
                Role = rolesList.FirstOrDefault() ?? "",
                Status = user.Status
            };
        }

        public virtual async Task<List<UserDto>> GetAllUser(bool? isActive = true)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (isActive == true)
            {
                usersQuery = usersQuery.Where(u => u.EmailConfirmed);
            }

            var users = await usersQuery
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var listUsersDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roleList = await _userManager.GetRolesAsync(user);

                listUsersDtos.Add(new UserDto()
                {
                    Id = user.Id,
                    Name = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    NumberIdentification = user.NumberIdentification,
                    isVerified = user.EmailConfirmed,
                    Role = roleList.FirstOrDefault() ?? "",
                    Status = user.Status,
                    CreatedAt = user.CreatedAt
                });
            }

            return listUsersDtos;
        }

        public virtual async Task<string> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "No existe una cuenta registrada con este usuario.";
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.Status = true;
                await _userManager.UpdateAsync(user);
                return $"Cuenta confirmada para {user.Email}. Ya puedes usar la aplicación.";
            }
            else
            {
                return $"Ocurrió un error al confirmar este correo electrónico: {user.Email}";
            }
        }

        public virtual async Task<UserResponseDto> CambiarEstadoAsync(string id)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No se encontró el usuario con ID: {id}.");
                return response;
            }

            user.Status = !user.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(e => e.Description).ToList());
            }

            return response;
        }

        public virtual async Task<UserResponseDto> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No se encontró el usuario con ID: {id}.");
                return response;
            }

            var emailOwner = await _userManager.FindByEmailAsync(dto.Email);
            if (emailOwner != null && emailOwner.Id != id)
            {
                response.HasError = true;
                response.Errors.Add($"El correo electrónico {dto.Email} ya está en uso por otro usuario.");
                return response;
            }

            var userNameOwner = await _userManager.FindByNameAsync(dto.UserName);
            if (userNameOwner != null && userNameOwner.Id != id)
            {
                response.HasError = true;
                response.Errors.Add($"El nombre de usuario {dto.UserName} ya está en uso por otro usuario.");
                return response;
            }

            var identificationOwner = await _userManager.Users.FirstOrDefaultAsync(u => u.NumberIdentification == dto.NumberIdentification && u.Id != id);
            if (identificationOwner != null)
            {
                response.HasError = true;
                response.Errors.Add($"El número de cédula {dto.NumberIdentification} ya está registrado por otro usuario.");
                return response;
            }

            user.FirstName = dto.Name;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.UserName;
            user.NumberIdentification = dto.NumberIdentification;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(updateResult.Errors.Select(e => e.Description).ToList());
                return response;
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            return response;
        }

        #region "Private methods"

        private async Task<string> GetVerificationEmailUri(AppUser user)
        {
            var token = await GetVerificationEmailToken(user);
            var route = "confirm-account";
            var completeUrl = new Uri(string.Concat(_appSettings.FrontendBaseUrl, "/", route));
            var verificationUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", token);
            return verificationUri;
        }

        private async Task<string> GetResetPasswordUri(AppUser user)
        {
            var token = await GetResetPasswordToken(user);
            var route = "reset-password";
            var completeUrl = new Uri(string.Concat(_appSettings.FrontendBaseUrl, "/", route));
            var resetUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            resetUri = QueryHelpers.AddQueryString(resetUri, "token", token);
            return resetUri;
        }

        private async Task<string> GetVerificationEmailToken(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        private async Task<string> GetResetPasswordToken(AppUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        #endregion
    }
}