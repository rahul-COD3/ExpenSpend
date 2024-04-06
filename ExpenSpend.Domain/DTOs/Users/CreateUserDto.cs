using ExpenSpend.Domain.Users.Const;
using System.ComponentModel.DataAnnotations;

namespace ExpenSpend.Domain.DTOs.Users;

public class CreateUserDto
{
    [RegularExpression(UserConsts.FirstNameRegex, ErrorMessage = UserConsts.FirstNameRegexErrorMessage)]
    public required string FirstName { get; set; }

    [RegularExpression(UserConsts.LastNameRegex, ErrorMessage = UserConsts.LastNameRegexErrorMessage)]
    public required string LastName { get; set; }

    [EmailAddress(ErrorMessage = UserConsts.EmailErrorMessage)]
    public required string Email { get; set; }

    [RegularExpression(UserConsts.PhoneNumberRegex, ErrorMessage = UserConsts.PhoneNumberRegexErrorMessage)]
    public string? PhoneNumber { get; set; }

    [RegularExpression(UserConsts.PasswordRegex, ErrorMessage = UserConsts.PasswordRegexErrorMessage)]
    public required string Password { get; set; }
}
