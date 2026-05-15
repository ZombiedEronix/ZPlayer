using AudioPlayer.API;
using DiscordRPC;
using ZPlayer.AudioEngine;

namespace AudioPlayer
{
    public static class DiscordPresence
    {
        public static DiscordRpcClient rpc;
        public static DateTime currentTime;

        static ServerTrackInfo currentTrackInfo;


        public static void Initialize()
        {
            if (rpc == null)
            {
                rpc = new("1499811653275881564");
                if (!rpc.IsInitialized)
                {
                    rpc.Initialize();
                }
                Player.Get().PlaybackStarted += Events.OnStartPlayBack;
                Player.Get().PlaybackStopped += Events.OnStopPlayBack;
            }
        }

        public static void SetTrackRPC(ServerTrackInfo info)
        {
            if (DateTime.Now >= currentTime + TimeSpan.FromSeconds(7))
                currentTime = DateTime.Now;
            if (rpc == null) return;
            
            currentTrackInfo = info;
            TimeSpan span = TimeSpan.FromMilliseconds(info.trackTimeMillis);
            rpc.SetPresence(new RichPresence()
            {
                Type = ActivityType.Listening,
                Details = info.Title,
                State = string.Join(", ", info.Artist),

                Assets = new Assets()
                {
                    LargeImageKey = info.ArtUrl,
                },
                
                Timestamps = new Timestamps
                {
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow.Add(span)
                },

                Buttons =  new Button[]
                 {
                     new() { Label = "AppleMusic", Url = info.TrackViewUrl }
                 } 
            });

            rpc.Invoke();
        }

        public static void PauseRPC()
        {
            ServerTrackInfo info = currentTrackInfo;

            if (rpc == null) return;
            
            currentTrackInfo = info;
            TimeSpan span = TimeSpan.FromMilliseconds(info.trackTimeMillis);
            rpc.SetPresence(new RichPresence()
            {
                Type = ActivityType.Listening,
                Details = info.Title,
                State = string.Join(", ", info.Artist),

                Assets = new Assets()
                {
                    LargeImageKey = info.ArtUrl,
                    SmallImageText = "Paused"
                },

                Buttons = new Button[]
                 {
                     new() { Label = "AppleMusic", Url = info.TrackViewUrl }
                 }
            });
        }
    }


    public static class Events
    {

        public static void OnPausePlayBack()
        {
            if (DiscordPresence.rpc != null)
            {
                DiscordPresence.PauseRPC();
            }
        }

        public static void OnStopPlayBack()
        {
            if (DiscordPresence.rpc != null)
            {
                DiscordPresence.rpc.ClearPresence();
            }
        }

        public static void OnStartPlayBack()
        {
            string title = Player.Get().CurrentTrackMetadata.Title;
            string artists = string.Join(", ", Player.Get().CurrentTrackMetadata.Artists);
            ITunes.GetResults(artists, title).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.Result.Count != 0)
                    {
                        DiscordPresence.SetTrackRPC(task.Result[0]);
                    }
                }
            });
        }
    }
}