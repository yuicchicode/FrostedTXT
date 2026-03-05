using FrostedTXT.App.Infrastructure.Utils;

namespace FrostedTXT.App.Models;

public sealed class AppSettings : ObservableObject
{
    private string _fontFamilyName = "Segoe UI";
    private double _fontSizeBase = 15;
    private double _zoomLevel = 1.0;
    private double _backgroundOpacity = 0.16;
    private string _tintColor = "#000000";
    private double _blurLevel = 16;
    private bool _wordWrapEnabled = true;
    private bool _restoreTabsOnStartup = true;

    public string FontFamilyName
    {
        get => _fontFamilyName;
        set => SetProperty(ref _fontFamilyName, value);
    }

    public double FontSizeBase
    {
        get => _fontSizeBase;
        set => SetProperty(ref _fontSizeBase, value);
    }

    public double ZoomLevel
    {
        get => _zoomLevel;
        set => SetProperty(ref _zoomLevel, value);
    }

    public double BackgroundOpacity
    {
        get => _backgroundOpacity;
        set => SetProperty(ref _backgroundOpacity, value);
    }

    public string TintColor
    {
        get => _tintColor;
        set => SetProperty(ref _tintColor, value);
    }

    public double BlurLevel
    {
        get => _blurLevel;
        set => SetProperty(ref _blurLevel, value);
    }

    public bool WordWrapEnabled
    {
        get => _wordWrapEnabled;
        set => SetProperty(ref _wordWrapEnabled, value);
    }

    public bool RestoreTabsOnStartup
    {
        get => _restoreTabsOnStartup;
        set => SetProperty(ref _restoreTabsOnStartup, value);
    }
}
