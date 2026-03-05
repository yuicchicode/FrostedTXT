using System;
using System.Runtime.InteropServices;

namespace FrostedTXT.App.Infrastructure.Interop;

internal static class User32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();
}
