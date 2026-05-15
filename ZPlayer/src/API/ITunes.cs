using System.Text.Json;

namespace AudioPlayer.API
{
    public struct ServerTrackInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtUrl { get; set; }

        public long trackTimeMillis;
        public string TrackViewUrl { get; set; }
    }

    public class ITunes
    {
        public static async Task<List<ServerTrackInfo>> GetResults(string artist, string track)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(artist + " " + track)}&entity=song&limit=1";

            var response = await client.GetStringAsync(url);
            using JsonDocument doc = JsonDocument.Parse(response);

            var root = doc.RootElement.GetProperty("results");
            List<ServerTrackInfo> info = new();

            for (int i = 0; i < root.GetArrayLength(); i++)
            {
                string? receivedTitle = root[0].GetProperty("trackName").GetString();
                long trackTime = root[0].GetProperty("trackTimeMillis").GetInt64();
                string? receivedArtist = root[0].GetProperty("artistName").GetString();
                string? artUrl = root[0].GetProperty("artworkUrl100").GetString();
                string? TrackViewUrl = root[0].GetProperty("trackViewUrl").GetString();


                info.Add(new ServerTrackInfo
                {
                    Title = receivedTitle != null ? receivedTitle : string.Empty,
                    Artist = receivedArtist != null ? receivedArtist : string.Empty,
                    trackTimeMillis = trackTime,
                    ArtUrl = artUrl != null ? artUrl : string.Empty,
                    TrackViewUrl = TrackViewUrl != null ? TrackViewUrl : string.Empty
                });
            }

            return info;
        }
    }
}