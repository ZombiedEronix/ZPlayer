using CSCore;
using ZPlugin.Interface;

namespace ZPlayer.AudioEngine
{
    public class ZPluginAggregator : SampleAggregatorBase
    {
        private readonly List<IZVST> plugins;

        public ZPluginAggregator(ISampleSource source, List<IZVST> plugins) : base(source)
        {
            base.BaseSource = source;
            this.plugins = plugins;
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            int required = count;
            for (int i = plugins.Count - 1; i >= 0; i--)
            {
                if(plugins[i].isEnabled)
                {
                    required = plugins[i].GetRequiredInput(required, base.WaveFormat.Channels);
                }
            }

            float[] current = new float[required];

            int read = base.Read(current, 0, required);

            if (read == 0) return count;

            foreach (var plugin in plugins)
            {
                if (plugin.isEnabled)
                {
                    int outputSize = plugin.GetOutputSize(current.Length, base.WaveFormat.Channels);
                    float[] outputData = new float[outputSize];

                    plugin.Process(current, outputData, base.WaveFormat.Channels);

                    current = outputData;
                }
            }

            Buffer.BlockCopy(current, 0, buffer, offset * sizeof(float), count * sizeof(float));
            return count;

        }
    }
}
