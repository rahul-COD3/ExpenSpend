using ExpenSpend.Domain.Models.Friends;
using Microsoft.AspNetCore.Identity;

namespace ExpenSpend.Domain.Models.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public double AmountOwed { get; set; } = 0;
    public double AmountOwes { get; set; } = 0;
}