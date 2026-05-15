using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ZPlayer.AudioEngine;

namespace AudioPlayer.AvaloniaApp
{
    public partial class PlayerWindow : Window
    {
        Player player = Player.Get();
        TextBlock? titleText;
        TextBlock? artistText;
        Slider? progressSlider;
        Image? imageControl;
        Image? backgroundImage;

        ListBox? playList;
        readonly DispatcherTimer timer;

        bool isDraggingProgressSlider = false;

        public PlayerWindow()
        {
            InitializeComponent();
            // Enable dragging the window by clicking on the title bar
            TitleBar.PointerPressed += (s, e) => this.BeginMoveDrag(e);

            // Find controls by their names defined in XAML
            backgroundImage = this.FindControl<Image>("BackgroundImage");
            imageControl = this.FindControl<Image>("AlbumArt");
            playList = this.FindControl<ListBox>("playlist");
            artistText = this.FindControl<TextBlock>("Artist_Txt");
            titleText = this.FindControl<TextBlock>("Title_Txt");
            progressSlider = this.FindControl<Slider>("ProgressSlider");

            // Attach event handlers
            progressSlider.AddHandler(PointerReleasedEvent, ProgressSliderReleased, RoutingStrategies.Tunnel);
            progressSlider.AddHandler(PointerPressedEvent, ProgressSliderPressed, RoutingStrategies.Tunnel);
            playList.AddHandler(DragDrop.DragOverEvent, OnPlaylistDragOver);
            playList.AddHandler(DragDrop.DropEvent, OnPlaylistDrop);

            // Set initial volume
            VolumeSlider.Value = AppSettings.volume;

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (s, e) => ProgressSliderUpdate();
            // Attach player events
            player.FileOpened += UpdateTitleText;
            player.FileOpened += ArtUpdate;
            player.PlaybackStarted += () => timer.Start();
            player.PlaybackStopped += () => timer.Stop();
            player.PlaylistUpdated += OnPlaylistUpdate;
            // Initialize the timer for updating the progress slider
        }


        /// <summary>
        /// Updates the title and artist text blocks with the current track's metadata when a new file is opened.
        /// </summary>
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

        /// <summary>
        /// Opens a file picker dialog to allow the user to select audio files and adds them to the player's playlist.
        /// </summary>
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
        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        protected override void OnClosing(WindowClosingEventArgs e)
        {
            player.Stop();
            base.OnClosing(e);
        }

        void OnUserDoubleTapToTrack(object? sender, Avalonia.Input.TappedEventArgs e)
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

        void ArtUpdate()
        {
            if (imageControl != null && backgroundImage != null)
            {
                var art = player.CurrentTrackMetadata.Art;
                imageControl.Source = art;
                backgroundImage.Source = art;
            }
        }

        void SetVolume(object? sender, RangeBaseValueChangedEventArgs e)
        {
            AppSettings.volume = (float)e.NewValue;
            if (player.engine.IsInitialized)
            {
                player.engine.Volume = (float)e.NewValue / 100F;
            }
        }

        private void ProgressSliderPressed(object? sender, PointerPressedEventArgs e)
        {
            isDraggingProgressSlider = true;
        }
        void ProgressSliderUpdate()
        {
            if (!isDraggingProgressSlider)
            {
                progressSlider.Value = (player.engine.GetPosition().TotalSeconds / player.engine.GetFileDuration().TotalSeconds) * 100;
                CurrentTime_Txt.Text = player.engine.GetPosition().ToString(@"mm\:ss");
                Duration_Txt.Text = player.engine.GetFileDuration().ToString(@"mm\:ss");
            }
        }
        void ProgressSliderReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            isDraggingProgressSlider = false;
            player.engine.SetPosition(TimeSpan.FromSeconds(player.engine.GetFileDuration().TotalSeconds * Math.Clamp(ProgressSlider.Value, 0, 100) / 100f));
            ProgressSliderUpdate();
        }

        internal void Stop()
        {
            timer.Stop();
            player.Stop();
        }

        void OnPlaylistDragOver(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataFormats().Contains(DataFormats.Files))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        void OnPlaylistDrop(object? sender, DragEventArgs e)
        {
            var items = e.Data.GetFiles();

            foreach (var item in items)
            {
                string? filePath = item.Path.LocalPath;

                if (!string.IsNullOrEmpty(filePath))
                {
                    player.AddTrackInPlaylist(filePath);
                }
            }
        }
        public void Play(object? sender, RoutedEventArgs e) => player.Play();
        void OnPlaylistUpdate() => playList.ItemsSource = player.TracksList.ToArray();
        void Next(object? sender, RoutedEventArgs e) => player.Next();
        void CloseWindow(object? sender, RoutedEventArgs e) => Close();
        void MinimizeWindow(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        void Prev(object? sender, RoutedEventArgs e) => player.Prev();
        void MaximazeWindow(object? sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState = WindowState.Normal : WindowState = WindowState.Maximized;
    }
}