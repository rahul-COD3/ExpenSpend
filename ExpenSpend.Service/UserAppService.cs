using AutoMapper;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Data.Context;
using ExpenSpend.Domain.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ExpenSpend.Repository.Contracts;
using ExpenSpend.Service.Models;
using ExpenSpend.Service.Contracts;

namespace ExpenSpend.Service;

public class UserAppService : IUserAppService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    public UserAppService(UserManager<ApplicationUser> userManager, IRepository<ApplicationUser> userRepository, IMapper mapper, ApplicationDbContext expenSpendDbContext, IHttpContextAccessor httpContext)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _context = expenSpendDbContext;
        _httpContext = httpContext;
    }

    public async Task<Response> GetLoggedInUser()
    {
        var loggedInUserId = _httpContext.HttpContext.User.Identity?.Name;
        if (loggedInUserId != null)
        {
            var user = await _userManager.FindByNameAsync(loggedInUserId);
            return new Response(_mapper.Map<GetUserDto>(user));
        }
        return null;
    }
    public async Task<Response> GetAllUsersAsync()
    {
        return new Response(_mapper.Map<List<GetUserDto>>(await _userRepository.GetAllAsync()));
    }
    public async Task<Response> GetUserByIdAsync(string id)
    {
        return new Response(await _userManager.FindByIdAsync(id));
    }
    public async Task<Response> GetUserByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user != null)
        {
            return new Response(_mapper.Map<GetUserDto>(user));
        }
        return new Response("User not found!");
    }
    public async Task<Response> UpdateUserAsync(Guid id, UpdateUserDto user)
    {
        var userToUpdate = await _userRepository.GetByIdAsync(id);
        if (userToUpdate == null)
        {
            return new Response("User not found!");
        }
        try
        {
            var updatedUser = _mapper.Map<ApplicationUser>(userToUpdate);
            await _userRepository.UpdateAsync(updatedUser);
            return new Response(_mapper.Map<GetUserDto>(updatedUser));
        }
        catch (Exception ex)
        {
            return new Response("Somthing went wrong!");
        }
    }

    public async Task<Response> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return new Response("User not found!");
        }
        await _userRepository.DeleteAsync(user);
        return new Response("User deleted successfully!");
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        if (result != null)
        {
            return result;
        }
        return null;
    }
}
