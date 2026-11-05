namespace Joker_app_backend.Models;

public class Joke
{
    public int Id { get; set; }
    public string Setup { get; set; } = string.Empty;
    public string PunchLine { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserID { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

}