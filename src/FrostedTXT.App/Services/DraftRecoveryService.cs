using FrostedTXT.App.Infrastructure.IO;
using FrostedTXT.App.Models;

namespace FrostedTXT.App.Services;

public sealed class DraftRecoveryService
{
    private readonly JsonFileStore _store;

    public DraftRecoveryService(JsonFileStore store)
    {
        _store = store;
    }

    public async Task SaveDraftAsync(NoteDocument document, CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();

        await AtomicFileWriter.WriteTextAsync(
            AppPaths.DraftTextPath(document.DraftId),
            document.Text,
            createBackup: false,
            cancellationToken).ConfigureAwait(false);

        var meta = new DraftMeta
        {
            DraftId = document.DraftId,
            FilePath = document.FilePath,
            DisplayTitle = document.DisplayName,
            UpdatedAtUtc = DateTime.UtcNow,
            LastAutoSavedAtUtc = DateTime.UtcNow
        };

        await _store.SaveAsync(AppPaths.DraftMetaPath(document.DraftId), meta, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<NoteDocument>> RestoreDraftTabsAsync(CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();
        var results = new List<NoteDocument>();

        foreach (var metaPath in Directory.EnumerateFiles(AppPaths.DraftsDir, "*.meta.json"))
        {
            var meta = await _store.LoadAsync(metaPath, () => new DraftMeta(), cancellationToken).ConfigureAwait(false);
            if (meta.DraftId == Guid.Empty)
            {
                continue;
            }

            var textPath = AppPaths.DraftTextPath(meta.DraftId);
            var text = File.Exists(textPath)
                ? await File.ReadAllTextAsync(textPath, cancellationToken).ConfigureAwait(false)
                : string.Empty;

            var displayName = !string.IsNullOrWhiteSpace(meta.DisplayTitle)
                ? meta.DisplayTitle
                : (!string.IsNullOrWhiteSpace(meta.FilePath) ? Path.GetFileName(meta.FilePath) : "Untitled");

            results.Add(new NoteDocument
            {
                DraftId = meta.DraftId,
                Text = text,
                FilePath = meta.FilePath,
                DisplayName = displayName,
                IsDirty = true,
                UpdatedAt = meta.UpdatedAtUtc == default ? DateTime.UtcNow : meta.UpdatedAtUtc,
                LastSavedAt = null
            });
        }

        return results;
    }

    public sealed class DraftMeta
    {
        public Guid DraftId { get; set; }
        public string? FilePath { get; set; }
        public string? DisplayTitle { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime LastAutoSavedAtUtc { get; set; }
    }
}
