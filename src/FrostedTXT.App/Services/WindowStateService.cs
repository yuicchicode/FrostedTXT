using System.Windows;
using System.Windows.Media;
using FrostedTXT.App.Infrastructure.Interop;

namespace FrostedTXT.App.Services;

public sealed class WindowStateService
{
    private bool _isWindowedFullscreen;
    private Rect _restoreBounds;
    private ResizeMode _restoreResizeMode;

    public bool IsWindowedFullscreen => _isWindowedFullscreen;

    public void ToggleWindowedFullscreen(Window window)
    {
        if (_isWindowedFullscreen)
        {
            ExitWindowedFullscreen(window);
            return;
        }

        EnterWindowedFullscreen(window);
    }

    public void ExitWindowedFullscreen(Window window)
    {
        if (!_isWindowedFullscreen)
        {
            return;
        }

        ExitWindowedFullscreenCore(window);
    }

    private void EnterWindowedFullscreen(Window window)
    {
        var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        var hasWorkArea = User32.TryGetMonitorWorkArea(handle, out var workArea);
        if (!hasWorkArea)
        {
            workArea = new User32.Rect
            {
                Left = (int)SystemParameters.WorkArea.Left,
                Top = (int)SystemParameters.WorkArea.Top,
                Right = (int)SystemParameters.WorkArea.Right,
                Bottom = (int)SystemParameters.WorkArea.Bottom
            };
        }

        var logicalWorkArea = ToLogicalRect(window, workArea);

        _restoreResizeMode = window.ResizeMode;
        _restoreBounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        window.WindowState = WindowState.Normal;
        window.ResizeMode = ResizeMode.NoResize;
        window.Left = logicalWorkArea.Left;
        window.Top = logicalWorkArea.Top;
        window.Width = logicalWorkArea.Width;
        window.Height = logicalWorkArea.Height;

        _isWindowedFullscreen = true;
    }

    private void ExitWindowedFullscreenCore(Window window)
    {
        window.WindowState = WindowState.Normal;
        window.ResizeMode = _restoreResizeMode;
        window.Left = _restoreBounds.Left;
        window.Top = _restoreBounds.Top;
        window.Width = _restoreBounds.Width;
        window.Height = _restoreBounds.Height;

        _isWindowedFullscreen = false;
    }

    private static Rect ToLogicalRect(Window window, User32.Rect deviceRect)
    {
        var source = PresentationSource.FromVisual(window);
        if (source?.CompositionTarget is null)
        {
            return new Rect(
                deviceRect.Left,
                deviceRect.Top,
                deviceRect.Right - deviceRect.Left,
                deviceRect.Bottom - deviceRect.Top);
        }

        var transform = source.CompositionTarget.TransformFromDevice;
        var topLeft = transform.Transform(new Point(deviceRect.Left, deviceRect.Top));
        var bottomRight = transform.Transform(new Point(deviceRect.Right, deviceRect.Bottom));

        return new Rect(topLeft, bottomRight);
    }
}
