using AudioPlayer.AvaloniaApp;
using Avalonia;
using System.Diagnostics;
using System.IO.Pipes;
namespace AudioPlayer;

internal static class Program
{
    public static Player player;
    [STAThread]
    public static void Main(string[] args)
    {
        if (StartPipeClient(args)) return;
        Task.Run(() => StartPipeServer("ZPlayer"));

        //===============================
        player = new();

        player.FileOpened += Events.OnStartPlayBack;

        //===============================
        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }


    public static bool StartPipeClient(string[] args)
    {
        var current = Process.GetCurrentProcess();
        var existing = Process.GetProcessesByName(current.ProcessName)
                                .FirstOrDefault(p => p.Id != current.Id);

        if (existing != null)
        {
            using (var client = new NamedPipeClientStream(".", "ZPlayer", PipeDirection.Out))
            {
                client.Connect(100);
                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(args[0]);
                }
            }
            return true;
        }
        return false;
    }

    private static void StartPipeServer(string v)
    {
        while (true)
        {
            using (var server = new NamedPipeServerStream("ZPlayer", PipeDirection.In))
            {
                server.WaitForConnection();
                using (var reader = new StreamReader(server))
                {
                    var filePath = reader.ReadLine();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            ((App)Application.Current).playerWindow.Stop();
                            player.ClearPlaylist();
                            player.AddTrackInPlaylist(filePath);
                            ((App)Application.Current).playerWindow.Play(null, null);
                        });
                    }
                }
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace();
    }
}
