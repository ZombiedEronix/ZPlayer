using System.Drawing;
using NAudio.Wave;
using ZPlugin.Interface;

namespace AudioPlayer
{
    public class PluginSampleProvider : ISampleProvider
    {
        public readonly ISampleProvider source;
        public List<IZPlugin> plugins;
        public PluginSampleProvider(ISampleProvider source, List<IZPlugin> plugins)
        {
            this.source = source;
            this.plugins = plugins;
        }

        public WaveFormat WaveFormat => source.WaveFormat;



        public int Read(float[] buffer, int offset, int count)
        {
            int required = count;
            for (int i = plugins.Count - 1; i >= 0; i--)
            {
                if(plugins[i].isEnabled)
                {
                    required = plugins[i].GetRequiredInput(required, source.WaveFormat.Channels);
                }
            }

            float[] current = new float[required];

            int read = source.Read(current, 0, required);

            if (read == 0) return count;

            foreach (var plugin in plugins)
            {
                if (plugin.isEnabled)
                {
                    int outputSize = plugin.GetOutputSize(current.Length, source.WaveFormat.Channels);
                    float[] outputData = new float[outputSize];

                    plugin.Process(current, outputData, source.WaveFormat.Channels);

                    current = outputData;
                }
            }

            Buffer.BlockCopy(current, 0, buffer, offset * sizeof(float), count * sizeof(float));
            return count;

        }
    }
}
