using FrostedTXT.App.Infrastructure.IO;
using FrostedTXT.App.Models;

namespace FrostedTXT.App.Services;

public sealed class DocumentService
{
    public async Task<NoteDocument> OpenAsync(string path, CancellationToken cancellationToken = default)
    {
        var text = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        return new NoteDocument
        {
            DraftId = Guid.NewGuid(),
            Text = text,
            FilePath = path,
            DisplayName = Path.GetFileName(path),
            IsDirty = false,
            UpdatedAt = DateTime.UtcNow,
            LastSavedAt = DateTime.UtcNow
        };
    }

    public async Task SaveAsync(NoteDocument document, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(document.FilePath))
        {
            throw new InvalidOperationException("FilePath is missing. Use Save As.");
        }

        await AtomicFileWriter.WriteTextAsync(document.FilePath, document.Text, createBackup: true, cancellationToken).ConfigureAwait(false);
        document.IsDirty = false;
        document.LastSavedAt = DateTime.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;
        document.DisplayName = Path.GetFileName(document.FilePath);
    }

    public async Task SaveAsAsync(NoteDocument document, string path, CancellationToken cancellationToken = default)
    {
        document.FilePath = path;
        document.DisplayName = Path.GetFileName(path);
        await SaveAsync(document, cancellationToken).ConfigureAwait(false);
    }
}
