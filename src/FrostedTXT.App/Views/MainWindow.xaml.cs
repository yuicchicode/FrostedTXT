using System.Windows;
using System.Windows.Input;
using FrostedTXT.App.Services;
using FrostedTXT.App.ViewModels;

namespace FrostedTXT.App.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly SettingsService _settingsService;
    private readonly FontCatalogService _fontCatalogService;
    private readonly WindowEffectsService _windowEffectsService;

    public MainWindow()
    {
        InitializeComponent();

        var jsonStore = new Infrastructure.IO.JsonFileStore();
        _settingsService = new SettingsService(jsonStore);
        var draftRecoveryService = new DraftRecoveryService(jsonStore);
        _fontCatalogService = new FontCatalogService();
        _windowEffectsService = new WindowEffectsService();

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

    private void TopBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximizeRestore();
            return;
        }

        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Minimize_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestore_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleMaximizeRestore();
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ToggleMaximizeRestore()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}
