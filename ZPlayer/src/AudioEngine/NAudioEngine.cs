using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Reflection;
using ZPlugin.Interface;

namespace AudioPlayer
{
    public class NAudioEngine : IAudioEngine
    {
        WaveOutEvent waveOut;
        AudioFileReader reader;


        List<IZPlugin> plugins = new();

        BufferedWaveProvider bufferedWave;

        PluginSampleProvider pluginProvider;

        VolumeSampleProvider provider;

        public float Volume
        {
            get => provider.Volume; set
            {
                provider.Volume = Math.Clamp(value, 0, 1);
            }
        }

        public bool IsInitialized { get => isInitialized; set => isInitialized = value; }
        public bool isPlaying { get; set; }
        public WaveFormat? WaveFormat
        {
            get
            {
                if (reader != null)
                {
                    return reader.WaveFormat;
                }
                else return null;
            }
        }

        bool isInitialized;
        private bool _isSeeking;
        public NAudioEngine()
        {
            waveOut = new();
            LoadPlugins();
        }

        public void LoadPlugins()
        {
            string pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            
            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

            var files = Directory.GetFiles(pluginDir, "*.dll");
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(IZPlugin).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var type in pluginTypes)
                    {
                        var plugin = (IZPlugin)Activator.CreateInstance(type);
                        Console.WriteLine($"Loaded plugin: {plugin.Name} by {plugin.Author}");
                        plugin.OpenSettings();
                        plugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Plugin loading error {file}: {ex.Message}");
                }
            }
        }
        public double GetPosition()
        {
            if (isInitialized)
            {
                return (reader.CurrentTime / reader.TotalTime) * 100f;
            }
            return 0;
        }

       

        public void OpenAudioFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (IsInitialized)
                {
                    Stop();
                }

                reader = new(filePath);
                bufferedWave = new(reader.WaveFormat);
                pluginProvider = new(bufferedWave.ToSampleProvider(), plugins);
                provider = new(pluginProvider);
                waveOut.Init(provider);

                Volume = AppSettings.volume / 100f;

                IsInitialized = true;
            }
        }

        void BufferUpdate()
        {
            byte[] buffer = new byte[1024 * 256];
            bufferedWave.DiscardOnBufferOverflow = true;
            while (isPlaying && IsInitialized)
            {
                if (bufferedWave.BufferedBytes < bufferedWave.BufferLength / 2)
                {
                    int read = reader.Read(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        bufferedWave.AddSamples(buffer, 0, read);
                    }
                }

                Thread.Sleep(20);
            }
        }


        public void Play()
        {
            if (IsInitialized)
            {
                if (isPlaying)
                {
                    waveOut.Pause();
                    isPlaying = false;
                }
                else
                {
                    waveOut.Play();
                    isPlaying = true;
                    Task.Run(BufferUpdate);
                }
            }
        }

        public void Stop()
        {
            waveOut.Stop();

            isPlaying = false;
            waveOut.Dispose();
            if (reader != null)
            {
                reader.Dispose();
            }
            isInitialized = false;
            bufferedWave = null;
        }

        public void SetPosition(double percents)
        {
            if (_isSeeking) return;

            _isSeeking = true;
            reader.CurrentTime = TimeSpan.FromSeconds(reader.TotalTime.TotalSeconds * Math.Clamp(percents, 0, 100) / 100f);
            bufferedWave.ClearBuffer();
            _isSeeking = false;
        }

        public void InitializeAudio()
        {
        }
    }
}