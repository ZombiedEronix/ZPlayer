namespace ZPlugin.Interface
{
    public interface IZPlugin
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        float Speed {get; set;}

        bool isEnabled { get; set; }
        
        int GetRequiredInput(int outputCount, int channels);

        int GetOutputSize(int inputCount, int channels);

        void OpenSettings();

        int Process(float[] input, float[] output, int channels);
    }
}
