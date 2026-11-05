namespace Joker_app_backend.DTOs;

public class JokeResponceDto
{
    public int Id { get; set; }
    public string Setup { get; set; } = string.Empty;
    public string PunchLine { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
}

