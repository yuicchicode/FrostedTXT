using FrostedTXT.App.Infrastructure.IO;

namespace FrostedTXT.App.Services;

public sealed class TabSessionService
{
    private readonly JsonFileStore _store;

    public TabSessionService(JsonFileStore store)
    {
        _store = store;
    }

    public Task<TabSession> LoadAsync(CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();
        return _store.LoadAsync(AppPaths.SessionFile, () => new TabSession(), cancellationToken);
    }

    public Task SaveAsync(TabSession session, CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();
        return _store.SaveAsync(AppPaths.SessionFile, session, cancellationToken);
    }

    public sealed class TabSession
    {
        public List<TabSessionItem> Tabs { get; set; } = new();
        public int SelectedIndex { get; set; }
    }

    public sealed class TabSessionItem
    {
        public Guid DraftId { get; set; }
        public string? FilePath { get; set; }
    }
}
