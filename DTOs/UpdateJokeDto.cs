using System.ComponentModel.DataAnnotations;

namespace Joker_app_backend.DTOs;

public class UpdateJokeDto
{
    [Required]
    [MaxLength(300)]
    public string Setup { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string PunchLine { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = "General";
}