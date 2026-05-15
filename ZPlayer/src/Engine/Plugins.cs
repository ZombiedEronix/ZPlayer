using System.Reflection;
using ZPlugin.Interface;

namespace ZPlayer.AudioEngine
{
    internal static class Plugins
    {
        static List<IZVST> plugins = new();
        static List<IZTheme> themes = new();

        static bool isLoaded = false;

        public static List<IZVST> GetAudioPlugins()
        {
            if(isLoaded) return plugins;
            else throw new PluginsNotLoadedException();
        }

        public static List<IZTheme> GetThemes()
        {
            if(isLoaded) return themes;
            else throw new PluginsNotLoadedException();
        }

        public static void LoadPlugins()
        {
            if(isLoaded) return;
            string pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

            string[] files = Directory.GetFiles(pluginDir, "*.dll");

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(IZVST).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var type in pluginTypes)
                    {
                        var plugin = (IZVST)Activator.CreateInstance(type);
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
            isLoaded = true;
        }
    }

    public class PluginsNotLoadedException : Exception
    {
        public PluginsNotLoadedException() : base("Plugins not loaded yet. Call LoadPlugins() first.") { }
    }
}
