using AudioPlayer.AvaloniaApp;
using Avalonia;
using System.Diagnostics;
using System.Windows.Forms;
using ZPlayer.AudioEngine;
namespace AudioPlayer;

internal static partial class Program
{
    public static Player player;
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var ex = (Exception)e.ExceptionObject;

            string errorArgs = $"\"{ex.Message}\" \"{ex.StackTrace}\"";
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string exePath = Path.Combine(baseDir, "ZPlayerCrashHandler.exe");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = errorArgs,
                    UseShellExecute = true
                });
            }
            catch {MessageBox.Show("Не удалось запустить обработчик сбоев.");}
            Environment.Exit(1);
        };

        if (Pipe.StartPipeClient(args)) return;
        Task.Run(() => Pipe.StartPipeServer("ZPlayer"));

        //===============================
        player = new();

        player.FileOpened += Events.OnStartPlayBack;

        //===============================
        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace();
    }
}
