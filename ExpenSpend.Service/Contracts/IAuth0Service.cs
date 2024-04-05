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
        Task<GetAuth0UserDto> GetUserInfo(string accessToken);
    }
}
