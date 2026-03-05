using System.Windows.Media;

namespace FrostedTXT.App.Services;

public sealed class FontCatalogService
{
    private IReadOnlyList<string>? _cache;

    public IReadOnlyList<string> GetInstalledFontFamilies()
    {
        if (_cache is not null)
        {
            return _cache;
        }

        _cache = Fonts.SystemFontFamilies
            .Select(f => f.Source)
            .OrderBy(n => n)
            .ToList();

        return _cache;
    }
}
