using FrostedTXT.App.Infrastructure.IO;
using FrostedTXT.App.Models;

namespace FrostedTXT.App.Services;

public sealed class SettingsService
{
    private readonly JsonFileStore _store;

    public SettingsService(JsonFileStore store)
    {
        _store = store;
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();
        var settings = await _store.LoadAsync(AppPaths.SettingsFile, () => new AppSettings(), cancellationToken).ConfigureAwait(false);

        settings.BackgroundOpacity = Math.Clamp(settings.BackgroundOpacity, 0.05, 0.45);
        if (settings.BackgroundOpacity > 0.28)
        {
            settings.BackgroundOpacity = 0.18;
        }

        if (string.IsNullOrWhiteSpace(settings.TintColor))
        {
            settings.TintColor = "#000000";
        }

        return settings;
    }

    public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        AppPaths.EnsureDirectories();
        return _store.SaveAsync(AppPaths.SettingsFile, settings, cancellationToken);
    }
}
