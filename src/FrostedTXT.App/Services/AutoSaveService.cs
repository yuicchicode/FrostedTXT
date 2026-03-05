using FrostedTXT.App.Infrastructure.Utils;
using FrostedTXT.App.Models;

namespace FrostedTXT.App.Services;

public sealed class AutoSaveService
{
    private readonly DraftRecoveryService _draftRecoveryService;
    private readonly Dictionary<Guid, Debouncer> _debouncers = new();
    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);

    public AutoSaveService(DraftRecoveryService draftRecoveryService)
    {
        _draftRecoveryService = draftRecoveryService;
    }

    public void Schedule(NoteDocument document, Action<DateTime>? onSaved = null)
    {
        if (!_debouncers.TryGetValue(document.DraftId, out var debouncer))
        {
            debouncer = new Debouncer(_delay);
            _debouncers[document.DraftId] = debouncer;
        }

        debouncer.Debounce(async () =>
        {
            await _draftRecoveryService.SaveDraftAsync(document).ConfigureAwait(false);
            onSaved?.Invoke(DateTime.UtcNow);
        });
    }
}
