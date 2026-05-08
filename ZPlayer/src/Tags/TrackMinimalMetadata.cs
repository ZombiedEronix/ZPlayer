using System.Drawing;

namespace AudioPlayer.Tags
{
    public struct TrackMinimalMetadata
    {

        public string TrackName { get; set; }
        public string[] Artists { get; set; }
        public TimeSpan Duration { get; set; }
        public Bitmap? CoverImage { get; set; }
        public string FilePath { get; set; }


        public string DurationDisplay
        {
            get
            {
                if (Duration.TotalHours >= 1)
                {
                    return Duration.ToString(@"hh\:mm\:ss");
                }
                return Duration.ToString(@"mm\:ss");
            }
        }
        public string TrackDisplay => (!string.IsNullOrEmpty(TrackName)) ? TrackName : "Untitled";
        public string ArtistDisplay => (Artists.Length > 0) ? string.Join(", ", Artists) : "Unknown";
    }
}