using AudioPlayer;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams;
using System.Windows.Forms;
using ZPlayer.AudioEngine;

namespace ZPlayer.src.Engine
{
    internal class CSCoreEngine : IAudioEngine
    {
        ISoundOut _out;
        private VolumeSource _volumeControl;
        IWaveSource _source;
        private bool isinitialized;

        public float Volume { get => _out.Volume; set => _out.Volume = value; }

        public bool IsInitialized { get => isinitialized; set => isinitialized = value; }

        public CSCoreEngine()
        {
            _out = new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100);
        }

        public void SetAudioOutputMode(AudioInputMode mode)
        {
            switch (mode)
            {
                case AudioInputMode.WasapiExclusive:
                    ChangeAudioOutput(new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Exclusive, 100));
                    break;
                case AudioInputMode.Wasapi:
                    ChangeAudioOutput(new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, 100));
                    break;
                case AudioInputMode.DirectSound:
                    ChangeAudioOutput(new DirectSoundOut());
                    break;
                case AudioInputMode.WaveOut:
                    ChangeAudioOutput(new WaveOut());
                    break;
            }
        }

        public void ChangeAudioOutput(ISoundOut newOutput)
        {
            if (isinitialized)
            {
                _out.Stop();
                _out.Dispose();
                _out = newOutput;
                _out.Initialize(_source);
                _out.Play();
            }
            else
            {
                _out.Dispose();
                _out = newOutput;
            }
        }

        public TimeSpan GetFileDuration()
        {
            if (_source != null || isinitialized)
            {
                return _source.GetLength();
            }
            return TimeSpan.Zero;
        }

        public TimeSpan GetPosition()
        {
            if (_source != null || isinitialized)
            {
                return _source.GetPosition();
            }
            return TimeSpan.Zero;
        }

        public void OpenAudioFile(string path)
        {
            Stop();

            try
            {
                ISampleSource codec = CodecFactory.Instance.GetCodec(path).ToSampleSource();
                ZPluginAggregator _aggregator = new(codec, Plugins.GetAudioPlugins());
                _volumeControl = new VolumeSource(_aggregator) { Volume = AppSettings.volume / 100F };
                _source = _volumeControl.ToWaveSource();
                _out.Initialize(_source);
                isinitialized = true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: {e.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void Play()
        {
            if (isinitialized) _out.Play();
        }
        public void SetPosition(TimeSpan position)
        {
            _source.SetPosition(position);
        }
        public void Stop()
        {
            if (isinitialized)
            {
                if (_out != null)
                {
                    _out.Stop();
                }
                if (_source != null)
                {
                    _source.Dispose();
                    _source = null;
                    isinitialized = false;
                }

            }
        }

        public void Pause()
        {
            if(IsInitialized)
            {
                _out.Pause();
            }
        }
    }
}
