using ExpenSpend.Domain.DTOs.Accounts;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenSpend.Service.Contracts
{
    public interface IAuthAppService
    {
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser? user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser? user);
        Task<string> GenerateResetToken(ApplicationUser? user);
        Task<SignInResult> LoginUserAsync(string email, string password);
        Task<JwtSecurityToken?> LoginUserJwtAsync(string userName, string password, bool rememberMe);
        Task<JwtSecurityToken?> LoginUserJwtAsync(ApplicationUser? user, bool rememberMe);
        Task LogoutUserAsync();
        Task<UserRegistrationResult> RegisterUserAsync(CreateUserDto input);
        Task<IdentityResult> RegisterUserAsync(ApplicationUser? user, string password);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser? user, string token, string newPassword);
    }
}
