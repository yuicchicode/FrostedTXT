using FrostedTXT.App.Infrastructure.Utils;
using FrostedTXT.App.Models;
using FrostedTXT.App.Services;

namespace FrostedTXT.App.ViewModels;

public sealed class DocumentTabViewModel : ObservableObject
{
    private readonly AutoSaveService _autoSaveService;
    private string _textContent;
    private string? _filePath;
    private string _title;
    private bool _isDirty;
    private DateTime? _lastAutoSavedAt;

    public DocumentTabViewModel(NoteDocument document, AutoSaveService autoSaveService, Func<DocumentTabViewModel, Task> saveAsync, Func<DocumentTabViewModel, Task> saveAsAsync, Action<DocumentTabViewModel> close)
    {
        _autoSaveService = autoSaveService;
        DraftId = document.DraftId;
        _textContent = document.Text;
        _filePath = document.FilePath;
        _title = string.IsNullOrWhiteSpace(document.DisplayName) ? "Untitled" : document.DisplayName;
        _isDirty = document.IsDirty;
        LastSavedAt = document.LastSavedAt;

        SaveCommand = new RelayCommand(async () => await saveAsync(this).ConfigureAwait(false));
        SaveAsCommand = new RelayCommand(async () => await saveAsAsync(this).ConfigureAwait(false));
        CloseCommand = new RelayCommand(() => close(this));
    }

    public Guid DraftId { get; }

    public string TextContent
    {
        get => _textContent;
        set
        {
            if (!SetProperty(ref _textContent, value))
            {
                return;
            }

            IsDirty = true;
            _autoSaveService.Schedule(ToDocument(), onSaved: utc =>
            {
                var localTime = utc.ToLocalTime();
                if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                {
                    LastAutoSavedAt = localTime;
                }
                else
                {
                    _ = System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LastAutoSavedAt = localTime);
                }
            });
        }
    }

    public string? FilePath
    {
        get => _filePath;
        set
        {
            if (SetProperty(ref _filePath, value))
            {
                Title = string.IsNullOrWhiteSpace(value) ? "Untitled" : Path.GetFileName(value);
            }
        }
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (SetProperty(ref _isDirty, value))
            {
                OnPropertyChanged(nameof(DisplayTitle));
            }
        }
    }

    public string DisplayTitle => IsDirty ? $"{Title}*" : Title;

    public DateTime? LastAutoSavedAt
    {
        get => _lastAutoSavedAt;
        set => SetProperty(ref _lastAutoSavedAt, value);
    }

    public DateTime? LastSavedAt { get; set; }

    public RelayCommand SaveCommand { get; }
    public RelayCommand SaveAsCommand { get; }
    public RelayCommand CloseCommand { get; }

    public NoteDocument ToDocument()
    {
        return new NoteDocument
        {
            DraftId = DraftId,
            Text = TextContent,
            FilePath = FilePath,
            DisplayName = Title,
            IsDirty = IsDirty,
            UpdatedAt = DateTime.UtcNow,
            LastSavedAt = LastSavedAt
        };
    }
}
