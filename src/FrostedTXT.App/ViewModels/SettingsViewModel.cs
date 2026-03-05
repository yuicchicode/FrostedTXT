using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using FrostedTXT.App.Infrastructure.Utils;
using FrostedTXT.App.Models;
using FrostedTXT.App.Services;

namespace FrostedTXT.App.ViewModels;

public sealed class SettingsViewModel : ObservableObject
{
    private readonly AppSettings _settings;
    private readonly SettingsService _settingsService;
    private readonly WindowEffectsService _effectsService;
    private readonly Window _window;

    public SettingsViewModel(AppSettings settings, SettingsService settingsService, FontCatalogService fontCatalogService, WindowEffectsService effectsService, Window window)
    {
        _settings = settings;
        _settingsService = settingsService;
        _effectsService = effectsService;
        _window = window;

        AvailableFonts = new ObservableCollection<string>(fontCatalogService.GetInstalledFontFamilies());
        SaveCommand = new RelayCommand(async () => await _settingsService.SaveAsync(_settings).ConfigureAwait(false));
    }

    public ObservableCollection<string> AvailableFonts { get; }

    public RelayCommand SaveCommand { get; }

    public string FontFamilyName
    {
        get => _settings.FontFamilyName;
        set { _settings.FontFamilyName = value; OnPropertyChanged(); }
    }

    public double FontSizeBase
    {
        get => _settings.FontSizeBase;
        set { _settings.FontSizeBase = value; OnPropertyChanged(); }
    }

    public double ZoomLevel
    {
        get => _settings.ZoomLevel;
        set { _settings.ZoomLevel = value; OnPropertyChanged(); }
    }

    public double BackgroundOpacity
    {
        get => _settings.BackgroundOpacity;
        set
        {
            _settings.BackgroundOpacity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TintBrush));
            ApplyWindowEffects();
        }
    }

    public string TintColor
    {
        get => _settings.TintColor;
        set
        {
            _settings.TintColor = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TintBrush));
            ApplyWindowEffects();
        }
    }

    public double BlurLevel
    {
        get => _settings.BlurLevel;
        set
        {
            _settings.BlurLevel = value;
            OnPropertyChanged();
            ApplyWindowEffects();
        }
    }

    public bool WordWrapEnabled
    {
        get => _settings.WordWrapEnabled;
        set
        {
            _settings.WordWrapEnabled = value;
            OnPropertyChanged();
        }
    }

    public Brush TintBrush => _effectsService.BuildTintBrush(_settings);

    public async Task PersistAsync()
    {
        ApplyWindowEffects();
        await _settingsService.SaveAsync(_settings).ConfigureAwait(false);
    }

    public void ApplyWindowEffects()
    {
        _effectsService.Apply(_window, _settings);
    }
}
