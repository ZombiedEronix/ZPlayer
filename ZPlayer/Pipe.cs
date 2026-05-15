using AudioPlayer.AvaloniaApp;
using Avalonia;
using System.Diagnostics;
using System.IO.Pipes;
using ZPlayer.AudioEngine;
namespace AudioPlayer;

internal static partial class Program
{
    public static class Pipe
    {
        public static bool StartPipeClient(string[] args)
        {
            var current = Process.GetCurrentProcess();
            var existing = Process.GetProcessesByName(current.ProcessName)
                                    .FirstOrDefault(p => p.Id != current.Id);

            if (existing != null)
            {
                using (var client = new NamedPipeClientStream(".", "ZPlayer", PipeDirection.Out))
                {
                    client.Connect(1000);
                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine(args.Length);
                        if (args.Length > 0)
                        {
                            for (int i = 0; i < args.Length; i++)
                            {
                                writer.WriteLine(args[i]);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static void StartPipeServer(string v)
        {
            while (true)
            {
                using (var server = new NamedPipeServerStream("ZPlayer", PipeDirection.In))
                {
                    server.WaitForConnection();
                    using (var reader = new StreamReader(server))
                    {
                        if (!int.TryParse(reader.ReadLine(), out int count)) continue;

                        var pathsToLoad = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            var path = reader.ReadLine();
                            if (!string.IsNullOrEmpty(path)) pathsToLoad.Add(path);
                        }

                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            Player player = Player.Get();

                            player.ClearPlaylist();
                            player.Stop();
                            foreach(var filepath in pathsToLoad)
                            {
                                player.AddTrackInPlaylist(filepath);
                            }
                            player.Play();


                            var app = (App)Application.Current;
                            if (app?.playerWindow == null) return;

                            Player.Get().ClearPlaylist();
                            app.playerWindow.Stop();

                            foreach (var filePath in pathsToLoad)
                            {
                                Player.Get().AddTrackInPlaylist(filePath);
                            }

                            app.playerWindow.Play(null, null);
                        });
                    }
                }
            }
        }
    }
}
