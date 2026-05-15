using Avalonia.Media.Imaging;

namespace AudioPlayer.Tags
{
    public static class TrackMetadataMethods
    {
        public static TrackMinimalMetadata GetTrack(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            if (!_supportedExtensions.Contains(extension))
            {
                return new()
                {
                    TrackName = Path.GetFileNameWithoutExtension(filePath),
                    Artists = Array.Empty<string>(),
                    FilePath = filePath,
                };
            }

            var file = TagLib.File.Create(filePath);
            TimeSpan timeSpan = file.Properties.Duration;


            Bitmap art = null;

            if (file.Tag.Pictures.Length > 0)
            {
                using (var ms = new MemoryStream(file.Tag.Pictures[0].Data.Data))
                {
                    var bitmap = new Bitmap(ms);

                    art = bitmap;
                }
            }

            TrackMinimalMetadata track = new TrackMinimalMetadata
            {
                TrackName = $"{file.Tag.Title}",
                Artists = file.Tag.AlbumArtists,
                FilePath = filePath,
                CoverImage = art != null ? art.CreateScaledBitmap(new(64, 64), BitmapInterpolationMode.HighQuality) : null,
                Duration = timeSpan
            };

            return track;
        }

        private static string[] _supportedExtensions = { ".mp3", ".wav", ".flac", ".ogg", ".m4a" };
        public static TrackFullMetadata GetMetaData(this TrackMinimalMetadata track)
        {
            var extension = Path.GetExtension(track.FilePath).ToLower();

            if (!_supportedExtensions.Contains(extension))
            {
                return new()
                {
                    Title = Path.GetFileNameWithoutExtension(track.FilePath),
                    FilePath = track.FilePath,
                };
            }

            TagLib.File file;

            try
            {
                file = TagLib.File.Create(track.FilePath);
            }
            catch
            {
                return new();
            }


            if (file == null)
            {
                return new();
            }

            var Name = file.Tag.Title;

            var artist = file.Tag.AlbumArtists;

            Bitmap? map = GetArt(track);

            return new TrackFullMetadata
            {
                Title = Name,
                Artists = artist,
                FilePath = track.FilePath,
                Art = map,
            };
        }

        public static Bitmap? GetArt(this TrackMinimalMetadata data)
        {
            var file = TagLib.File.Create(data.FilePath);

            if (file == null)
            {
                return null;
            }

            Bitmap art = null;

            if (file.Tag.Pictures.Length > 0)
            {
                using (var ms = new MemoryStream(file.Tag.Pictures[0].Data.Data))
                {
                    var bitmap = new Bitmap(ms);

                    art = bitmap;
                }
            }

            return art;

        }
    }
}