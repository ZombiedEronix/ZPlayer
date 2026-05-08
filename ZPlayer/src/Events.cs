using AudioPlayer.API;
using DiscordRPC;

namespace AudioPlayer
{
    public static class Events
    {
        static Player player = Program.player;
        public static DiscordRpcClient rpc;

        public static DateTime currentTime;


        public static void SetRPC(ServerTrackInfo info)
        {
            if(DateTime.Now >= currentTime + TimeSpan.FromSeconds(7))
            currentTime = DateTime.Now;
            if (rpc == null)
            {
                rpc = new("1499811653275881564");
                if (!rpc.IsInitialized)
                {

                    rpc.Initialize();
                }
            }

            
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
                // Buttons = new Button[]
                // {
                //     new() { Label = "Website", Url = "https://zeldadev.eu/" }
                // }
            });

            rpc.Invoke();
        }

        public static void OnStartPlayBack()
        {
            string title = player.CurrentTrackMetadata.Title;
            string artists = string.Join(", ", player.CurrentTrackMetadata.Artists);
            ITunes.GetResults(artists, title).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.Result.Count != 0)
                    {
                        SetRPC(task.Result[0]);
                    }
                }
            });

        }
    }
}