using System.Diagnostics;
using System.Media;
using System.Reflection;

namespace CrashHandler
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process duplicatedprocess = Process.GetProcessesByName("ZPlayerCrashHandler").FirstOrDefault();
            if (duplicatedprocess != null && duplicatedprocess.Id != currentProcess.Id)
            {
                duplicatedprocess.Kill();
            }




            if (args.Length < 2)
#if DEBUG
            {
                PlayCrashSound();
                Application.Run(new ErrorForm("", ""));
                return;
            }
#else
                return;
#endif
            {
                PlayCrashSound();
                Application.Run(new ErrorForm(args[0], args[1]));
            }
        }

        public static void PlayCrashSound()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = "ZPlayerCrashHandler.Resources.zvuk-litvina.wav";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                {
                    using (SoundPlayer player = new SoundPlayer(stream))
                    {
                        player.Play();
                        //Task.Run(() =>
                        //{
                        PlayGhoul();
                        //});
                    }
                }
            }
        }


        public static void PlayGhoul()
        {
            string[] songs = { "drowning_love.mid" };
            string targetFile = songs[new Random().Next(songs.Length)];

            string tempMidiPath = Path.Combine(Path.GetTempPath(), targetFile);
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = $"ZPlayerCrashHandler.Resources.{targetFile}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return;
                }
                try
                {
                    using (FileStream fileStream = File.Create(tempMidiPath))
                    {
                        MidiPlayer.Play(stream);
                    }
                }
                catch (IOException) { /* Файл уже существует и занят, просто играем его */ }
            }
        }
    }
}