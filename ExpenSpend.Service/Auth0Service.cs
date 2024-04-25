using ExpenSpend.Domain.DTOs.Accounts;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using ExpenSpend.Service.Contracts;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace ExpenSpend.Service
{
    public class Auth0Service : IAuth0Service
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthAppService _authAppService;
        private readonly IUserAppService _userAppService;
        private readonly HttpClient _client;
        private readonly string _auth0Domain;

        public Auth0Service(IConfiguration configuration, IAuthAppService authAppService, IUserAppService userAppService, HttpClient client)
        {
            _configuration = configuration;
            _authAppService = authAppService;
            _userAppService = userAppService;
            _client = client;
            _auth0Domain = _configuration["Auth0:Domain"];
        }

        public async Task<GetAuth0UserDto> GetUserInfo(string accessToken)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _client.GetAsync($"https://{_auth0Domain}/userinfo");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var userinfo = System.Text.Json.JsonSerializer.Deserialize<GetAuth0UserDto>(content);

                var user = await _userAppService.GetUserByEmailAsync(userinfo.Email);
                if (user == null)
                {
                    user = await RegisterNewUser(userinfo);
                }

                var userToken = await _authAppService.LoginUserJwtAsync(user, false);
                userinfo.JwtLoginToken = new JwtSecurityTokenHandler().WriteToken(userToken).ToString();
                return userinfo;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return null;
            }
        }

        private async Task<ApplicationUser> RegisterNewUser(GetAuth0UserDto userinfo)
        {
            var names = userinfo.Name!.Split(" ");
            var createUserDto = new CreateUserDto
            {
                Email = userinfo.Email,
                FirstName = names[0],
                LastName = names[1],
                Password = GenerateSecurePassword(), // Implement a secure password generation method
            };
            await _authAppService.RegisterUserAsync(createUserDto);
            return await _userAppService.GetUserByEmailAsync(userinfo.Email);
        }

        private string GenerateSecurePassword()
        {
            // Implement a secure password generation method
            return "SecurePassword";
        }
    }
}
