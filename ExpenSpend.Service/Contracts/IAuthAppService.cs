using ExpenSpend.Domain.DTOs.Accounts;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenSpend.Service.Contracts
{
    public interface IAuthAppService
    {
        /// <summary>
        /// Confirms the email for a user asynchronously.
        /// </summary>
        /// <param name="user">The user to confirm the email for.</param>
        /// <param name="token">The confirmation token.</param>
        /// <returns>An IdentityResult representing the outcome of the email confirmation operation.</returns>
        Task<IdentityResult> ConfirmEmailAsync(ApplicationUser? user, string token);

        /// <summary>
        /// Generates an email confirmation token for a user asynchronously.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The email confirmation token.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser? user);

        /// <summary>
        /// Generates a password reset token for a user asynchronously.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The password reset token.</returns>
        Task<string> GenerateResetToken(ApplicationUser? user);

        /// <summary>
        /// Logs in a user asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A SignInResult representing the outcome of the login operation.</returns>
        Task<SignInResult> LoginUserAsync(string email, string password);

        /// <summary>
        /// Logs in a user and generates a JWT token asynchronously.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="rememberMe">Flag indicating whether to remember the user.</param>
        /// <returns>A JWT token if login is successful, otherwise null.</returns>
        Task<JwtSecurityToken?> LoginUserJwtAsync(string userName, string password, bool rememberMe);

        /// <summary>
        /// Logs in a user and generates a JWT token asynchronously.
        /// </summary>
        /// <param name="user">The user to generate token for.</param>
        /// <param name="rememberMe">Flag indicating whether to remember the user.</param>
        /// <returns>A JWT token if login is successful, otherwise null.</returns>
        Task<JwtSecurityToken?> LoginUserJwtAsync(ApplicationUser? user, bool rememberMe);

        /// <summary>
        /// Logs out the current user asynchronously.
        /// </summary>
        Task LogoutUserAsync();

        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        /// <param name="input">The user information to register.</param>
        /// <returns>A result indicating the success or failure of the registration operation.</returns>
        Task<UserRegistrationResult> RegisterUserAsync(CreateUserDto input);

        /// <summary>
        /// Registers a user asynchronously.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>An IdentityResult representing the outcome of the registration operation.</returns>
        Task<IdentityResult> RegisterUserAsync(ApplicationUser? user, string password);

        /// <summary>
        /// Resets the password for a user asynchronously.
        /// </summary>
        /// <param name="user">The user to reset the password for.</param>
        /// <param name="token">The reset token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>An IdentityResult representing the outcome of the reset password operation.</returns>
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser? user, string token, string newPassword);
    }
}
