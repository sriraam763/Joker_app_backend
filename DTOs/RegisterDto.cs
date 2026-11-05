using System.ComponentModel.DataAnnotations;

namespace Joker_app_backend.DTOs;

public class RegisterDto
{
    [Required] public string FullName { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be atleast 6 characters.")]
    public string Password { get; set; } = string.Empty;
}