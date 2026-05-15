using AudioPlayer.AvaloniaApp;
using Avalonia;
using System.Diagnostics;
using ZPlayer.AudioEngine;
namespace AudioPlayer
{
    internal static partial class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (Pipe.StartPipeClient(args)) return;
            Task.Run(() => Pipe.StartPipeServer("ZPlayer"));
            
            Player.Initialize();
            MediaKeyControl.Initialize(Player.Get());
            DiscordPresence.Initialize();
            Plugins.LoadPlugins();



            //====================
            ConnectCrashHandler();
            try
            {
                BuildAvaloniaApp().LogToTrace().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                ExecuteCrashHandler(ex);
            }
            //====================
        }

        public static void ExecuteCrashHandler(Exception ex)
        {
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
            catch { Environment.Exit(1); }
        }

        public static void ConnectCrashHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = (Exception)e.ExceptionObject;
                ExecuteCrashHandler(ex);
                Environment.Exit(1);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                var ex = (Exception)e.Exception;
                ExecuteCrashHandler(ex);
                Environment.Exit(1);
            };
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                             .UseWin32()
                             .UseSkia()
                             .LogToTrace();
        }
    }
}