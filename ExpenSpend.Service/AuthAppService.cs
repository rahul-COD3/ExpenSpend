using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

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
        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser? user, string password)
        {
            return await _userManager.CreateAsync(user, password);
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
                    var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);

                    if (!isPasswordCorrect)
                    {
                        return new Response("Oops! It seems your credentials are incorrect. Please double-check your password and try again.");
                    }

                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
                    var emailConfirmationLink = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}/api/auth/confirm-email?token={emailConfirmationToken}&email={user.Email}";
                    var emailMessage = await _emailService.CreateEmailValidationTemplateMessage(user?.Email!, emailConfirmationLink!);
                    _emailService.SendEmail(emailMessage);
                    return new Response("Uh-oh! It looks like your email hasn't been confirmed yet. Don't worry, we've just sent you a new confirmation email. Please verify your email before logging in.");
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
    }
}
