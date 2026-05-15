using Avalonia.Media.Imaging;

public struct TrackFullMetadata
{
    public string Title { get; set; }
    public string[] Artists { get; set; }
    public int Bitrate { get; set; }
    //==================================
    public int SampleSize { get; set; }
    public int SampleRate { get; set; }
    //==================================
    public string Container { get; set; }
    public string format { get; set; }
    public Bitmap Art { get; set; }
    public uint Samples { get; set; }
    public string FilePath { get; set; }
    public string TrackDisplay => (!string.IsNullOrEmpty(Title)) ? Title : "Untitled";
    public string ArtistDisplay => (Artists.Length > 0) ? string.Join(", ", Artists) : "Unknown";
}
