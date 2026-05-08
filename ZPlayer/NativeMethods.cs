using System.Diagnostics;
using System.Runtime.InteropServices;
namespace AudioPlayer;

static class NativeMethods
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public static bool ActivateFirstInstance()
    {
        var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);


        if (processes.Length > 1)
        {
            var hWnd = processes[0].MainWindowHandle;

            if (hWnd == IntPtr.Zero)
            {
                SetForegroundWindow(IntPtr.Zero);

            }
            return true;
        }
        return false;
    }

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

}
