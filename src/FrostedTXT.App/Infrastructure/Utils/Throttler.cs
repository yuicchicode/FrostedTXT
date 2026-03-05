namespace FrostedTXT.App.Infrastructure.Utils;

public sealed class Throttler
{
    private readonly TimeSpan _interval;
    private DateTime _lastExecution = DateTime.MinValue;

    public Throttler(TimeSpan interval)
    {
        _interval = interval;
    }

    public bool TryEnter()
    {
        var now = DateTime.UtcNow;
        if (now - _lastExecution < _interval)
        {
            return false;
        }

        _lastExecution = now;
        return true;
    }
}
