using System.Windows;
using System.Windows.Media;
using FrostedTXT.App.Infrastructure.Interop;
using FrostedTXT.App.Models;
using FrostedTXT.App.Models.Enums;

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

        if (window.AllowsTransparency)
        {
            DwmApi.TryApplySystemBackdrop(handle, backdropType: 1);
            User32Accent.TryDisable(handle);
            return;
        }

        var mode = settings.BlurMode;
        if (mode == BlurMode.None)
        {
            DwmApi.TryApplySystemBackdrop(handle, backdropType: 1);
            User32Accent.TryDisable(handle);
            return;
        }

        var tintColor = ParseColor(settings.TintColor);
        var alpha = (byte)(Math.Clamp(settings.BackgroundOpacity, 0.0, 1.0) * 255);

        if (mode == BlurMode.Auto)
        {
            if (User32Accent.TrySetBlur(handle, acrylicLike: true, alpha, tintColor.R, tintColor.G, tintColor.B))
            {
                return;
            }

            if (DwmApi.TryEnableBlurBehind(handle))
            {
                return;
            }

            DwmApi.TryApplySystemBackdrop(handle, backdropType: 3);
            return;
        }

        if (mode == BlurMode.Blur)
        {
            if (!User32Accent.TrySetBlur(handle, acrylicLike: false, alpha, tintColor.R, tintColor.G, tintColor.B))
            {
                if (!DwmApi.TryEnableBlurBehind(handle))
                {
                    DwmApi.TryApplySystemBackdrop(handle, backdropType: 2);
                }
            }

            return;
        }

        if (!User32Accent.TrySetBlur(handle, acrylicLike: true, alpha, tintColor.R, tintColor.G, tintColor.B))
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
