using System.Runtime.InteropServices;

namespace FrostedTXT.App.Infrastructure.Interop;

internal static class DwmApi
{
    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    private const uint DWM_BB_ENABLE = 0x00000001;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    private static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DwmBlurBehind pBlurBehind);

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

    [StructLayout(LayoutKind.Sequential)]
    private struct DwmBlurBehind
    {
        public uint dwFlags;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fEnable;
        public IntPtr hRgnBlur;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fTransitionOnMaximized;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    public static bool TryApplySystemBackdrop(IntPtr hwnd, int backdropType)
    {
        try
        {
            var value = backdropType;
            var hr = DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
            return hr == 0;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryEnableBlurBehind(IntPtr hwnd)
    {
        try
        {
            var blur = new DwmBlurBehind
            {
                dwFlags = DWM_BB_ENABLE,
                fEnable = true,
                hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = true
            };

            return DwmEnableBlurBehindWindow(hwnd, ref blur) == 0;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryExtendFrameIntoClientArea(IntPtr hwnd)
    {
        try
        {
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1
            };

            return DwmExtendFrameIntoClientArea(hwnd, ref margins) == 0;
        }
        catch
        {
            return false;
        }
    }
}
