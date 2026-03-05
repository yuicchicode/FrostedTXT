using System.Runtime.InteropServices;

namespace FrostedTXT.App.Infrastructure.Interop;

internal static class User32Accent
{
    private const int WCA_ACCENT_POLICY = 19;

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public int Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    public static bool TrySetBlur(IntPtr hwnd, bool acrylicLike, byte alpha, byte red, byte green, byte blue)
    {
        var accent = new AccentPolicy
        {
            AccentState = acrylicLike ? 4 : 3,
            AccentFlags = 2,
            GradientColor = (alpha << 24) | (blue << 16) | (green << 8) | red,
            AnimationId = 0
        };

        var size = Marshal.SizeOf(accent);
        var accentPtr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WCA_ACCENT_POLICY,
                Data = accentPtr,
                SizeOfData = size
            };

            return SetWindowCompositionAttribute(hwnd, ref data) != 0;
        }
        catch
        {
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(accentPtr);
        }
    }

    public static bool TryDisable(IntPtr hwnd)
    {
        var accent = new AccentPolicy
        {
            AccentState = 0,
            AccentFlags = 0,
            GradientColor = 0,
            AnimationId = 0
        };

        var size = Marshal.SizeOf(accent);
        var accentPtr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WCA_ACCENT_POLICY,
                Data = accentPtr,
                SizeOfData = size
            };

            return SetWindowCompositionAttribute(hwnd, ref data) != 0;
        }
        catch
        {
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
