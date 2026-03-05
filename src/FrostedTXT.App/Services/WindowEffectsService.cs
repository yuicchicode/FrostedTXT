using System.Windows;
using System.Windows.Media;
using FrostedTXT.App.Infrastructure.Interop;
using FrostedTXT.App.Models;

namespace FrostedTXT.App.Services;

public sealed class WindowEffectsService
{
    public void Apply(Window window, AppSettings settings)
    {
        var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        if (settings.BlurLevel <= 0.01)
        {
            DwmApi.TryApplySystemBackdrop(handle, backdropType: 1);
            User32Accent.TryDisable(handle);
            return;
        }

        var level = Math.Clamp(settings.BlurLevel, 0.0, 40.0);
        var strength = level / 40.0;

        var tintColor = ParseColor(settings.TintColor);
        var tintOpacity = Math.Clamp(settings.BackgroundOpacity, 0.0, 1.0);
        var alpha = (byte)(tintOpacity * 255);
        var accentAlpha = (byte)Math.Clamp(alpha + (strength * 80), 24, 220);

        // For transparent window mode, prefer accent since DWM backdrop is not reliable there.
        if (window.AllowsTransparency)
        {
            if (User32Accent.TrySetBlur(handle, acrylicLike: true, accentAlpha, tintColor.R, tintColor.G, tintColor.B))
            {
                return;
            }

            if (!User32Accent.TrySetBlur(handle, acrylicLike: false, accentAlpha, tintColor.R, tintColor.G, tintColor.B))
            {
                DwmApi.TryEnableBlurBehind(handle);
            }

            return;
        }

        DwmApi.TryExtendFrameIntoClientArea(handle);

        if (!User32Accent.TrySetBlur(handle, acrylicLike: true, accentAlpha, tintColor.R, tintColor.G, tintColor.B))
        {
            if (!DwmApi.TryEnableBlurBehind(handle))
            {
                DwmApi.TryApplySystemBackdrop(handle, backdropType: 3);
            }
        }
    }

    public Brush BuildTintBrush(AppSettings settings)
    {
        var color = ParseColor(settings.TintColor);
        var opacity = Math.Clamp(settings.BackgroundOpacity, 0.0, 1.0);
        return new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B));
    }

    private static Color ParseColor(string colorHex)
    {
        try
        {
            return (Color)ColorConverter.ConvertFromString(colorHex);
        }
        catch
        {
            return (Color)ColorConverter.ConvertFromString("#141414");
        }
    }
}
