
using ZPlugin.Interface;

namespace ZPlugin.BitCrush
{
    public class BitCrushPlugin : IZPlugin
    {
        public string Name => "BitCrush";

        public string Author => "ZombiedEronix";

        public string Description => "A bitcrush plugin for audio processing";

        public string Version => "1.0.0";

        public int BitDepth = 8;
        
        public int SampleHold = 5;

        private float[] lastSamples = new float[2];

        private int holdCounter = 0;

        public float Speed { get; set; } = 1;
        public bool isEnabled { get; set; } = false;

        public int GetOutputSize(int inputCount, int channels)
        {
            return inputCount;
        }

        public int GetRequiredInput(int outputCount, int channels)
        {
            return outputCount;
        }

        public void OpenSettings()
        {

        }

        public int Process(float[] input, float[] output, int channels)
        {
            float levels = (float)Math.Pow(2, BitDepth);

            for (int i = 0; i < input.Length; i += channels)
            {
                if (holdCounter >= SampleHold)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        lastSamples[ch] = input[i + ch];
                        holdCounter = 0;
                    }
                }
                holdCounter++;

                for (int ch = 0; ch < channels; ch++)
                {
                    float sample = lastSamples[ch];

                    sample = (float)Math.Floor(sample * levels) / levels;

                    output[i + ch] = sample;
                }
            }
            return input.Length;
        }
    }
}
