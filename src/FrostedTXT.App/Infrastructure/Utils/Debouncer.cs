namespace FrostedTXT.App.Infrastructure.Utils;

public sealed class Debouncer
{
    private readonly TimeSpan _delay;
    private CancellationTokenSource? _cts;

    public Debouncer(TimeSpan delay)
    {
        _delay = delay;
    }

    public void Debounce(Func<Task> action)
    {
        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_delay, token).ConfigureAwait(false);
                if (!token.IsCancellationRequested)
                {
                    await action().ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }
}
