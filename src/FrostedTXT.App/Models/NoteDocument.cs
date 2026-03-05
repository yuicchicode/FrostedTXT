namespace FrostedTXT.App.Models;

public sealed class NoteDocument
{
    public Guid DraftId { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string DisplayName { get; set; } = "Untitled";
    public bool IsDirty { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSavedAt { get; set; }
}
