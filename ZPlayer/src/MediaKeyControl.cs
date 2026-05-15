using AudioPlayer;
using AudioPlayer.AvaloniaApp;
using Avalonia.Media.Imaging;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using ZPlayer.AudioEngine;

public static class MediaKeyControl
{
    private static MediaPlayer _dummyPlayer;
    private static Player player;
    private static bool isInitialized = false;
    
    public static void Initialize(Player player)
    {
        if (isInitialized)
            return;
        MediaKeyControl.player = player;
        _dummyPlayer = new MediaPlayer();


        var smtc = _dummyPlayer.SystemMediaTransportControls;
        smtc.IsEnabled = true;
        smtc.IsPlayEnabled = true;
        smtc.IsPauseEnabled = true;
        smtc.IsNextEnabled = true;
        smtc.IsPreviousEnabled = true;
        smtc.ButtonPressed += Smtc_ButtonPressed;
        player.PlaybackStarted += UpdateStatus;
        player.FileOpened += UpdateStatus;

        isInitialized = true;
    }

    private static void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
            case SystemMediaTransportControlsButton.Pause:

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    Player player = Player.Get();
                    player.Play();
                });
                break;
            case SystemMediaTransportControlsButton.Next:
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    Player player = Player.Get();
                    player.Next();
                });
                break;
            case SystemMediaTransportControlsButton.Previous:
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    Player player = Player.Get();
                    player.Prev();
                });
                break;
        }
        UpdateStatus();
    }

    public static async Task UpdateThumbnailFromBitmap(Bitmap avaloniaBitmap)
    {
        var updater = _dummyPlayer.SystemMediaTransportControls.DisplayUpdater;

        var ms = new InMemoryRandomAccessStream();

        using (var outputStream = new MemoryStream())
        {
            avaloniaBitmap.Save(outputStream);
            outputStream.Position = 0;

            using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(outputStream.ToArray());
                await writer.StoreAsync();
                await writer.FlushAsync();
            }
        }

        updater.Thumbnail = RandomAccessStreamReference.CreateFromStream(ms);
        updater.Update();
    }

    public static void UpdateStatus()
    {
        if (!isInitialized) return;
        var smtc = _dummyPlayer.SystemMediaTransportControls;
        smtc.PlaybackStatus = player.IsPlaying
            ? MediaPlaybackStatus.Playing
            : MediaPlaybackStatus.Paused;
        UpdateMetadata(player.CurrentTrackMetadata.TrackDisplay, player.CurrentTrackMetadata.ArtistDisplay);
        Task.Run(async () =>
        {
            if (player.CurrentTrackMetadata.Art != null)
            {
                await UpdateThumbnailFromBitmap(player.CurrentTrackMetadata.Art);
            }
        });
    }

    public static void Stop()
    {
        if (!isInitialized) return;
        var smtc = _dummyPlayer.SystemMediaTransportControls;
        smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
        isInitialized = false;
    }

    static void UpdateMetadata(string title, string author)
    {
        if (!isInitialized) return;
        var displayUpdater = _dummyPlayer.SystemMediaTransportControls.DisplayUpdater;
        displayUpdater.Type = MediaPlaybackType.Music;
        displayUpdater.MusicProperties.Title = title;
        displayUpdater.MusicProperties.Artist = author;
        displayUpdater.Update();
    }
}