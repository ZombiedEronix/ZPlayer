
using Avalonia.Controls;

namespace ZPlugin.Interface
{
    public interface IZTheme
    {
        public string Name { get; }

        //public string RequiredVersion { get; }
        public string Description { get; }
        ResourceDictionary GetResources();
        void InitializeTheme();
    }
}
