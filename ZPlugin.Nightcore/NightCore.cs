using ZPlugin.Interface;
namespace ZPlugin.Nightcore
{
    public class NightCore : IZPlugin
    {
        public float Speed { get; set; } = 1;

        public bool Interpolation { get; set; } = true;

        public string Name => "NightCore plugin";

        public string Author => "ZombiedEronix";

        public string Description => "A nightcore plugin for audio processing";

        public string Version => "1.0.0";

        public bool isEnabled { get; set; } = true;

        public int GetRequiredInput(int outputCount, int channels)
        {
            double raw = outputCount * Speed;

            int req = (int)Math.Ceiling(raw);

            if (req % channels != 0) req++;

            return req;
        }

        public int GetOutputSize(int inputCount, int channels)
        {
            double raw = inputCount / Speed;

            int req = (int)Math.Ceiling(raw);

            if (req % channels != 0) req++;

            return req;
        }

        public void OpenSettings()
        {
            var form = new Form
            {
                Text = "Настройки Nightcore",
                Width = 300,
                Height = 200,
                StartPosition = FormStartPosition.CenterScreen
            };

            var trackBar = new TrackBar { Minimum = 7, Maximum = 13, Value = (int)(10 * Speed), Dock = DockStyle.Top };
            var label = new Label { Text = $"Скорость: {Speed}", Dock = DockStyle.Top };

            trackBar.Scroll += (s, e) =>
            {
                float newSpeed = trackBar.Value / 10f;
                this.Speed = newSpeed;
                label.Text = $"Скорость: {newSpeed}";
            };

            form.Controls.Add(trackBar);
            form.Controls.Add(label);
            Task.Run(() =>
            {
                form.ShowDialog();
            });
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public int Process(float[] input, float[] output, int channels)
        {
            if (Speed == 1.0f)
            {
                Array.Copy(input, 0, output, 0, Math.Min(input.Length, output.Length));
                return input.Length;
            }

            int limit = output.Length;

            for (int i = 0; i < output.Length; i += 2)
            {
                float realIdx = i * Speed;
                int sourceIdx = (int)realIdx;
                sourceIdx -= (sourceIdx % 2);
                if (Interpolation)
                {

                    float t = (realIdx - sourceIdx) / 2f;

                    if (sourceIdx + 3 < input.Length)
                    {
                        output[i] = Lerp(input[sourceIdx], input[sourceIdx + 2], t);
                        output[i + 1] = Lerp(input[sourceIdx + 1], input[sourceIdx + 3], t);
                    }
                    else if (sourceIdx + 1 < input.Length)
                    {
                        output[i] = input[sourceIdx];
                        output[i + 1] = input[sourceIdx + 1];
                    }
                }
                else
                {
                    if (sourceIdx + 1 < input.Length)
                    {
                        output[i] = input[sourceIdx];
                        output[i + 1] = input[sourceIdx + 1];
                    }
                }
            }

            return limit;
        }
    }
}
