using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using FrostedTXT.App.Services;
using FrostedTXT.App.ViewModels;

namespace FrostedTXT.App.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly SettingsService _settingsService;
    private readonly FontCatalogService _fontCatalogService;
    private readonly WindowEffectsService _windowEffectsService;
    private readonly WindowStateService _windowStateService;

    public MainWindow()
    {
        InitializeComponent();

        var jsonStore = new Infrastructure.IO.JsonFileStore();
        _settingsService = new SettingsService(jsonStore);
        var draftRecoveryService = new DraftRecoveryService(jsonStore);
        _fontCatalogService = new FontCatalogService();
        _windowEffectsService = new WindowEffectsService();
        _windowStateService = new WindowStateService();

        _viewModel = new MainViewModel(
            new DocumentService(),
            draftRecoveryService,
            new AutoSaveService(draftRecoveryService),
            _settingsService,
            new DialogService(),
            new TabSessionService(jsonStore));

        _viewModel.RequestClose = Close;
        _viewModel.RequestOpenSettings = OpenSettings;
        _viewModel.RequestOpenAbout = OpenAbout;
        _viewModel.RequestToggleFullscreenWindowed = ToggleWindowedFullscreen;

        DataContext = _viewModel;

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await _viewModel.InitializeAsync();
            _windowEffectsService.Apply(this, _viewModel.Settings);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Falha ao inicializar o FrostedTXT.\n\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        await _viewModel.ShutdownAsync();
    }

    private void OpenSettings()
    {
        var vm = new SettingsViewModel(_viewModel.Settings, _settingsService, _fontCatalogService, _windowEffectsService, this);
        var window = new Dialogs.SettingsWindow
        {
            Owner = this,
            DataContext = vm
        };

        window.ShowDialog();
        _windowEffectsService.Apply(this, _viewModel.Settings);
    }

    private void OpenAbout()
    {
        var window = new Dialogs.AboutWindow
        {
            Owner = this,
            DataContext = new AboutViewModel()
        };

        window.ShowDialog();
    }

    private void HeaderRow_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || IsInteractiveTarget(e.OriginalSource as DependencyObject))
        {
            return;
        }

        if (e.ClickCount == 2)
        {
            ToggleWindowedFullscreen();
            e.Handled = true;
            return;
        }

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            if (_windowStateService.IsWindowedFullscreen)
            {
                _windowStateService.ExitWindowedFullscreen(this);
            }

            try
            {
                DragMove();
                e.Handled = true;
            }
            catch
            {
                // DragMove can throw if mouse button state changes mid-call.
            }
        }
    }

    private void Minimize_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestore_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleWindowedFullscreen();
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleWindowedFullscreen()
    {
        _windowStateService.ToggleWindowedFullscreen(this);
    }

    private static bool IsInteractiveTarget(DependencyObject? source)
    {
        while (source is not null)
        {
            if (source is ButtonBase
                || source is TextBoxBase
                || source is ComboBox
                || source is Slider
                || source is ScrollBar
                || source is TabItem
                || source is MenuItem)
            {
                return true;
            }

            source = VisualTreeHelper.GetParent(source);
        }

        return false;
    }
}
