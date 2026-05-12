using System.Media;

namespace CrashHandler
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            PlayCrashSound();
            Application.Run(new ErrorForm("", ""));
#else

            if (args.Length < 2) return;
            else
            {
                PlayCrashSound();
                Application.Run(new ErrorForm(args[0], args[1]));
            }
#endif
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
                    }
                }
            }
        }
    }
}