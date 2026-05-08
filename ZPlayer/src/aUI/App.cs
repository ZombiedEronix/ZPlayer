using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
namespace AudioPlayer.AvaloniaApp
{
    public partial class App : Avalonia.Application
    {
        public App()
        {
        }
        public PlayerWindow playerWindow {get; set;}

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                playerWindow = new PlayerWindow();
                desktop.MainWindow = playerWindow;

                if (desktop.Args != null && desktop.Args.Length == 1)
                {
                    string filePath = desktop.Args[0];
                    Program.player.AddTrackInPlaylist(filePath);
                    Program.player.Play();
                }

            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}