﻿using System.ComponentModel.DataAnnotations;

namespace ExpenSpend.Domain.DTOs.Accounts;

public class LoginDto
{
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 8 and 15 characters and contain at least one uppercase letter, one lowercase letter, one digit and one special character.")]
    public required string Password { get; set; }

    [Required]
    public bool RememberMe { get; set; }
}
