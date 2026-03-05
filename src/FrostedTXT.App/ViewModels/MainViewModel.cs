using System.Collections.ObjectModel;
using System.Windows;
using FrostedTXT.App.Infrastructure.Utils;
using FrostedTXT.App.Models;
using FrostedTXT.App.Services;

namespace FrostedTXT.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly DocumentService _documentService;
    private readonly DraftRecoveryService _draftRecoveryService;
    private readonly AutoSaveService _autoSaveService;
    private readonly SettingsService _settingsService;
    private readonly DialogService _dialogService;
    private readonly TabSessionService _sessionService;

    private DocumentTabViewModel? _selectedTab;

    public MainViewModel(
        DocumentService documentService,
        DraftRecoveryService draftRecoveryService,
        AutoSaveService autoSaveService,
        SettingsService settingsService,
        DialogService dialogService,
        TabSessionService sessionService)
    {
        _documentService = documentService;
        _draftRecoveryService = draftRecoveryService;
        _autoSaveService = autoSaveService;
        _settingsService = settingsService;
        _dialogService = dialogService;
        _sessionService = sessionService;

        Tabs = new ObservableCollection<DocumentTabViewModel>();

        NewTabCommand = new RelayCommand(NewTab);
        OpenCommand = new RelayCommand(async () => await OpenAsync().ConfigureAwait(false));
        SaveCommand = new RelayCommand(async () => await SaveSelectedAsync().ConfigureAwait(false), () => SelectedTab is not null);
        SaveAsCommand = new RelayCommand(async () => await SaveAsSelectedAsync().ConfigureAwait(false), () => SelectedTab is not null);
        CloseTabCommand = new RelayCommand(p => CloseTab(p as DocumentTabViewModel ?? SelectedTab), _ => Tabs.Count > 0);
        CloseAppCommand = new RelayCommand(() => RequestClose?.Invoke());
        OpenSettingsCommand = new RelayCommand(() => RequestOpenSettings?.Invoke());
        OpenAboutCommand = new RelayCommand(() => RequestOpenAbout?.Invoke());
    }

    public ObservableCollection<DocumentTabViewModel> Tabs { get; }

    public DocumentTabViewModel? SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (SetProperty(ref _selectedTab, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
                SaveAsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public AppSettings Settings { get; private set; } = new();

    public RelayCommand NewTabCommand { get; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand SaveAsCommand { get; }
    public RelayCommand CloseTabCommand { get; }
    public RelayCommand CloseAppCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand OpenAboutCommand { get; }

    public Action? RequestClose { get; set; }
    public Action? RequestOpenSettings { get; set; }
    public Action? RequestOpenAbout { get; set; }

    public async Task InitializeAsync()
    {
        Settings = await _settingsService.LoadAsync();
        OnPropertyChanged(nameof(Settings));

        if (Settings.RestoreTabsOnStartup)
        {
            await RestoreTabsAsync();
        }

        if (Tabs.Count == 0)
        {
            NewTab();
        }
    }

    public async Task ShutdownAsync()
    {
        var session = new TabSessionService.TabSession
        {
            SelectedIndex = SelectedTab is null ? 0 : Tabs.IndexOf(SelectedTab),
            Tabs = Tabs.Select(t => new TabSessionService.TabSessionItem
            {
                DraftId = t.DraftId,
                FilePath = t.FilePath
            }).ToList()
        };

        await _sessionService.SaveAsync(session);

        foreach (var tab in Tabs)
        {
            await _draftRecoveryService.SaveDraftAsync(tab.ToDocument());
        }
    }

    public void NewTab()
    {
        var doc = new NoteDocument
        {
            DraftId = Guid.NewGuid(),
            DisplayName = "Untitled",
            Text = string.Empty,
            IsDirty = false
        };

        var tab = CreateTab(doc);
        Tabs.Add(tab);
        SelectedTab = tab;
        CloseTabCommand.RaiseCanExecuteChanged();
    }

    public async Task OpenAsync()
    {
        var path = _dialogService.ShowOpenFileDialog();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var doc = await _documentService.OpenAsync(path);
        var tab = CreateTab(doc);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Tabs.Add(tab);
            SelectedTab = tab;
            CloseTabCommand.RaiseCanExecuteChanged();
        });

        await _draftRecoveryService.SaveDraftAsync(tab.ToDocument());
    }

    public async Task SaveSelectedAsync()
    {
        if (SelectedTab is null)
        {
            return;
        }

        await SaveTabAsync(SelectedTab);
    }

    public async Task SaveAsSelectedAsync()
    {
        if (SelectedTab is null)
        {
            return;
        }

        await SaveTabAsAsync(SelectedTab);
    }

    private async Task SaveTabAsync(DocumentTabViewModel tab)
    {
        var doc = tab.ToDocument();

        if (string.IsNullOrWhiteSpace(doc.FilePath))
        {
            await SaveTabAsAsync(tab);
            return;
        }

        await _documentService.SaveAsync(doc);
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            tab.IsDirty = false;
            tab.LastSavedAt = doc.LastSavedAt?.ToLocalTime();
            tab.FilePath = doc.FilePath;
            tab.Title = doc.DisplayName;
        });

        await _draftRecoveryService.SaveDraftAsync(doc);
    }

    private async Task SaveTabAsAsync(DocumentTabViewModel tab)
    {
        var suggested = string.IsNullOrWhiteSpace(tab.FilePath) ? "Untitled.txt" : Path.GetFileName(tab.FilePath);
        var path = _dialogService.ShowSaveFileDialog(suggested);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var doc = tab.ToDocument();
        await _documentService.SaveAsAsync(doc, path);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            tab.IsDirty = false;
            tab.LastSavedAt = doc.LastSavedAt?.ToLocalTime();
            tab.FilePath = doc.FilePath;
            tab.Title = doc.DisplayName;
        });

        await _draftRecoveryService.SaveDraftAsync(doc);
    }

    private void CloseTab(DocumentTabViewModel? tab)
    {
        if (tab is null)
        {
            return;
        }

        Tabs.Remove(tab);
        if (SelectedTab == tab)
        {
            SelectedTab = Tabs.LastOrDefault();
        }

        if (Tabs.Count == 0)
        {
            NewTab();
        }

        CloseTabCommand.RaiseCanExecuteChanged();
    }

    private async Task RestoreTabsAsync()
    {
        var session = await _sessionService.LoadAsync();
        var draftDocs = await _draftRecoveryService.RestoreDraftTabsAsync();

        var byId = draftDocs.ToDictionary(d => d.DraftId, d => d);
        var ordered = new List<NoteDocument>();

        foreach (var item in session.Tabs)
        {
            if (byId.TryGetValue(item.DraftId, out var doc))
            {
                if (!string.IsNullOrWhiteSpace(item.FilePath))
                {
                    doc.FilePath = item.FilePath;
                    doc.DisplayName = Path.GetFileName(item.FilePath);
                }

                ordered.Add(doc);
                byId.Remove(item.DraftId);
            }
        }

        ordered.AddRange(byId.Values);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (var doc in ordered)
            {
                Tabs.Add(CreateTab(doc));
            }

            SelectedTab = Tabs.Count == 0
                ? null
                : Tabs[Math.Clamp(session.SelectedIndex, 0, Tabs.Count - 1)];
        });
    }

    private DocumentTabViewModel CreateTab(NoteDocument doc)
    {
        return new DocumentTabViewModel(doc, _autoSaveService, SaveTabAsync, SaveTabAsAsync, CloseTab);
    }
}
