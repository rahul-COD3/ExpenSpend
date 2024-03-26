using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Users;
using ExpenSpend.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenSpend.Service.Contracts
{
    public interface IUserAppService
    {
        Task<Response> DeleteUserAsync(Guid id);
        Task<Response> GetAllUsersAsync();
        Task<Response> GetLoggedInUser();
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<Response> GetUserByIdAsync(string id);
        Task<Response> GetUserByUserNameAsync(string userName);
        Task<Response> UpdateUserAsync(Guid id, UpdateUserDto user);
    }
}
