using ExpenSpend.Domain.DTOs.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenSpend.Service.Contracts
{
    public interface IAuth0Service
    {
        /// <summary>
        /// Get user info from Auth0 and create user if not exists 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns> GetAuth0UserDto </returns>
        Task<GetAuth0UserDto> GetUserInfo(string accessToken);
    }
}
