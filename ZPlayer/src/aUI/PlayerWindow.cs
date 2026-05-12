using AudioPlayer.Tags;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System.Diagnostics;
using ZPlayer.AudioEngine;
namespace AudioPlayer.AvaloniaApp
{
    public partial class PlayerWindow : Window
    {
        Player player = Program.player;
        TextBlock? titleText;
        TextBlock? artistText;
        Slider? progressSlider;
        ListBox? playList;
        Image? imageControl;
        Image? background;


        ListBox? list;

        private readonly DispatcherTimer timer;

        public PlayerWindow()
        {
            InitializeComponent();


            TitleBar.PointerPressed += (s,e) =>
            {
                this.BeginMoveDrag(e);
            };

            background = this.FindControl<Image>("BackgroundImage");
            imageControl = this.FindControl<Image>("AlbumArt");
            list = this.FindControl<ListBox>("playlist");
            artistText = this.FindControl<TextBlock>("Artist_Txt");
            titleText = this.FindControl<TextBlock>("Title_Txt");
            progressSlider = this.FindControl<Slider>("ProgressSlider");
            playList = this.FindControl<ListBox>("Playlist");

            progressSlider.AddHandler(PointerReleasedEvent, ProgressSliderReleased, RoutingStrategies.Tunnel);

            VolumeSlider.Value = AppSettings.volume;

            player.FileOpened += UpdateTitleText;
            player.FileOpened += ArtUpdate;
            player.PlaybackStarted += () => timer.Start();
            player.PlaybackStopped += () => timer.Stop();
            player.PlaylistUpdated += OnPlaylistUpdate;
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };

            timer.Tick += ProgressSliderUpdate;

        }

        void UpdateTitleText()
        {
            if (titleText != null)
            {
                titleText.Text = player.CurrentTrackMetadata.TrackDisplay;
            }
            if (artistText != null)
            {
                artistText.Text = player.CurrentTrackMetadata.ArtistDisplay;
            }
        }

        async void SelectFiles(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Files",
                    AllowMultiple = true,
                    FileTypeFilter = new[] { new FilePickerFileType("Audio Files") { Patterns = new[] { "*.flac", "*.mp3", "*.wav" } } }
                });

                if (files.Count >= 1)
                {
                    foreach (var file in files)
                    {
                        string path = file.Path.LocalPath;
                        player.AddTrackInPlaylist(path);
                    }
                }
            }
        }


        protected override void OnClosing(WindowClosingEventArgs e)
        {
            player.Stop();
            base.OnClosing(e);
        }

        private void OnUserDoubleTapToTrack(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem != null)
            {
                if (player.TrackNumber == listBox.SelectedIndex) return;
                player.TrackNumber = listBox.SelectedIndex;
                player.Stop();
                player.Play();
            }
        }

        public void Play(object? sender, RoutedEventArgs e)
        {

            player.Play();
        }

        private void ArtUpdate()
        {
            if (imageControl != null && background != null)
            {
                var art = player.CurrentTrackMetadata.Art;
                if (art != null)
                {
                    Console.WriteLine($"Setting art: {art.Size.Width}x{art.Size.Height}");
                }
                else
                {
                    Console.WriteLine("Error: tracksList[0].Art is null!");
                }
                imageControl.Source = art;
                background.Source = art;
            }
            else
            {
                Console.WriteLine("Error: Image is not found or list is clean.");
            }
        }

        private void SetVolume(object? sender, RangeBaseValueChangedEventArgs e)
        {
            AppSettings.volume = (float)e.NewValue;
            if (player.engine.IsInitialized)
            {
                player.engine.Volume = (float)e.NewValue / 100F;
            }
        }


        private void ProgressSliderUpdate(object? sender, EventArgs e)
        {
            progressSlider.Value = player.engine.GetPosition();
        }
        private void OnPlaylistUpdate() => list.ItemsSource = player.TracksList.ToArray();

        private void ProgressSliderReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            player.engine.SetPosition(ProgressSlider.Value);
        }

        internal void Stop()
        {
            timer.Stop();
            player.Stop();
        }

        private void OnPlaylistDragOver(object? sender, DragEventArgs e)
        {
            if (e.DataTransfer.Formats.Contains(DataFormat.File))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        private void OnPlaylistDrop(object? sender, DragEventArgs e)
        {
            var items = e.DataTransfer.TryGetFiles();

                foreach (var item in items)
                {
                    string? filePath = item.Path.LocalPath;

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        player.AddTrackInPlaylist(filePath);
                    }
            }
        }

        private void Next(object? sender, RoutedEventArgs e)
        {
            player.Next();
        }

        private void Prev(object? sender, RoutedEventArgs e)
        {
            player.Prev();
        }
    }
}