namespace ZPlayer.AudioEngine
{
    public enum AudioInputMode
    {
        DirectSound,
        WaveOut,
        Wasapi,
        WasapiExclusive,
    }

    public interface IAudioEngine
    {
        public float Volume { get; set;}
        public void SetAudioOutputMode(AudioInputMode mode);
        public TimeSpan GetFileDuration();
        public bool IsInitialized { get; set;}
        public TimeSpan GetPosition();
        public void OpenAudioFile(string path);
        public void Stop();
        public void Play();
        public void Pause();
        public void SetPosition(TimeSpan position);
    }
}