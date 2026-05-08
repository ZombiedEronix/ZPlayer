using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace AudioPlayer
{
    public interface IAudioEngine
    {
        public float Volume { get; set;}

        public bool isPlaying { get; set;}
        public bool IsInitialized { get; set;}


        public double GetPosition();

        public void OpenAudioFile(string path);

        public void LoadPlugins();
        public void Stop();
        public void Play();

        public void SetPosition(double percents);
        void InitializeAudio();
    }
}