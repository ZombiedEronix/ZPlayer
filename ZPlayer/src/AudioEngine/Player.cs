using AudioPlayer.Tags;
namespace ZPlayer.AudioEngine
{
    public class Player
    {
        public readonly IAudioEngine engine = new NAudioEngine();
        List<TrackMinimalMetadata> tracksList = new();

        public TrackFullMetadata CurrentTrackMetadata { get; private set; }

        public event Action PlaybackStarted;
        public event Action FileOpened;
        public event Action PlaybackStopped;

        public event Action PlaylistUpdated;


        private int trackNumber;
        public int TrackNumber
        {
            get => trackNumber;
            set
            {
                trackNumber = Math.Clamp(value, 0, TracksList.Count - 1);
            }
        }

        public List<TrackMinimalMetadata> TracksList
        {
            get => tracksList;
            set
            {
                tracksList = value;
            }
        }

        public void OpenAudioFile()
        {
            string path = TracksList[TrackNumber].FilePath;
            engine.OpenAudioFile(path);
            CurrentTrackMetadata = TrackMetadataMethods.GetMetaData(TracksList[TrackNumber]);
            FileOpened?.Invoke();
        }

        public void Next()
        {
            TrackNumber++;
            OpenAudioFile();
            Play();
        }

        public void Prev()
        {
            TrackNumber--;
            OpenAudioFile();
            Play();
        }

        public void Stop()
        {
            engine.Stop();
            PlaybackStopped?.Invoke();
        }

        public void Play()
        {
            if (!engine.IsInitialized)
            {
                engine.InitializeAudio();
                if (TracksList.Count > 0)
                {
                    OpenAudioFile();
                }
            }
            engine.Play();
            PlaybackStarted?.Invoke();
        }

        public void AddTrackInPlaylist(string file)
        {
            tracksList.Add(TrackMetadataMethods.GetTrack(file));

            List<string> s = new();
            foreach (var it in tracksList)
            {
                string artistsText = string.Join(", ", it.Artists);
                if (it.Artists.Length != 0)
                {
                    s.Add($"{artistsText} - {it.TrackName}");
                }
                else
                {
                    s.Add($"{it.TrackName}");
                }
                PlaylistUpdated?.Invoke();
            }
        }

        public void ClearPlaylist()
        {
            trackNumber = 0;
            TracksList.Clear();
        }
    }
}