namespace FrostedTXT.App.Models;

public sealed class RecentFile
{
    public string Path { get; set; } = string.Empty;
    public DateTime LastOpenedAt { get; set; } = DateTime.UtcNow;
}
