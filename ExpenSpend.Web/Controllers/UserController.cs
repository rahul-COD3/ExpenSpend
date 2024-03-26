using System;
using System.Threading.Tasks;
using AutoMapper;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using ExpenSpend.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenSpend.Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserAppService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserAppService userRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _userService = userRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            var user = await _userService.GetLoggedInUser();
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(_mapper.Map<GetUserDto>(user.Data));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user.IsSuccess)
            {
               return Ok(_mapper.Map<GetUserDto>(user.Data));
            }
            return NotFound("User not found");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto input)
        {
            var result = await _userService.UpdateUserAsync(id, input);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result.Data);
        }
    }
}
