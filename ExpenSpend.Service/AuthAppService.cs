using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using ExpenSpend.Domain.DTOs.Accounts;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using ExpenSpend.Service.Contracts;
using ExpenSpend.Service.Emails.Interfaces;
using ExpenSpend.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpenSpend.Service
{
    public class AuthAppService : IAuthAppService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthAppService(
            IHttpContextAccessor contextAccessor,
            IMapper mapper,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
        )
        {
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<UserRegistrationResult> RegisterUserAsync(CreateUserDto input)
        {
            var userExists = await _userManager.FindByNameAsync(input.Email);
            if (userExists != null)
            {
                return UserRegistrationResult.UserExistsError();
            }

            var user = _mapper.Map<ApplicationUser>(input);
            user.UserName = input.Email;
            var registrationResult = await RegisterUserAsync(user, input.Password);

            if (registrationResult.Succeeded)
            {
                return UserRegistrationResult.Success();
            }
            else
            {
                return UserRegistrationResult.Failure<List<IdentityError>>(registrationResult.Errors.ToList());
            }

        }
        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if(!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    return IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "Oops! Looks like there's already an account with this email. Please log in or use a different email to sign up." });
                }
                return IdentityResult.Failed(result.Errors.ToArray());
            }
            await _userManager.AddToRoleAsync(user, "User");
            return result;
        }
        public async Task<SignInResult> LoginUserAsync(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(email, password, false, false);
        }
        public async Task<Response> LoginUserJwtAsync(string userName, string password, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return new Response("Oops! We couldn't find your account. Please double-check your credentials or consider signing up for a new account if you haven't already.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);

            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    if (!await _userManager.CheckPasswordAsync(user, password))
                    {
                        return new Response("Oops! It seems your credentials are incorrect. Please double-check your password and try again.");
                    }
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        return new Response("EMAIL_IS_NOT_VERIFIED");
                    }
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return new Response("Your account has been locked out. Please try again later or reset your password.");
                    }
                    return new Response("Oops! It seems there was an issue with your account. Please try again later or contact support.");
                }

                return new Response("Oops! It seems the email or password you entered is incorrect. Please double-check both and try again.");
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("FirstName",user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            authClaims.AddRange((await _userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));
            var expirationTime = rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(8);
            var token = GenerateTokenOptions(authClaims, expirationTime);

            return new Response(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }


        // set claims for user and generate token
        public async Task<JwtSecurityToken?> LoginUserJwtAsync(ApplicationUser? user, bool rememberMe)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("FirstName",user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            authClaims.AddRange((await _userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));
            var expirationTime = rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(8);
            return GenerateTokenOptions(authClaims, expirationTime);
        }
        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser? user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser? user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser? user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }
        public async Task<string> GenerateResetToken(ApplicationUser? user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        // Private method to generate JWT token options...
        private JwtSecurityToken? GenerateTokenOptions(List<Claim> authClaims, DateTime expires)
        {

            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);
            var tokenOptions = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: authClaims,
                        expires: expires,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return tokenOptions;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
