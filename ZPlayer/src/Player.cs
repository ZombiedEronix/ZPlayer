using AudioPlayer.Tags;
using ZPlayer.src.Engine;
namespace ZPlayer.AudioEngine
{
    public class Player
    {
        static Player instance;
        public readonly IAudioEngine engine = new CSCoreEngine();
        List<TrackMinimalMetadata> tracksList = new();

        public TrackFullMetadata CurrentTrackMetadata { get; private set; }

        public event Action PlaybackStarted;
        public event Action FileOpened;
        public event Action PlaybackStopped;
        public event Action PlaylistUpdated;

        private int trackNumber;
        internal bool IsPlaying;

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
            Stop();
            OpenAudioFile();
            Play();
        }

        public void Prev()
        {
            TrackNumber--;
            Stop();
            OpenAudioFile();
            Play();
        }

        public void Stop()
        {
            engine.Stop();
            IsPlaying = false;

            PlaybackStopped?.Invoke();
        }

        public void Play()
        {
            if (!engine.IsInitialized)
            {
                if (TracksList.Count > 0)
                {
                    OpenAudioFile();
                }
            }

            if(!IsPlaying)
            {
                engine.Play();
                IsPlaying = true;
                PlaybackStarted?.Invoke();
            }
            else
            {
                engine.Pause();
                IsPlaying = false;
                PlaybackStopped?.Invoke();
            }
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

        internal static Player Get()
        {
            if (instance == null) throw new Exception("Player is not initialized!");
            return instance;
        }

        internal static Player Initialize() => instance = new();
    }
}