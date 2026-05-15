
using System.Diagnostics;

namespace CrashHandler
{
    public partial class ErrorForm : Form
    {
        public ErrorForm(string message, string stackTrace)
        {
            InitializeComponent();
            if(string.IsNullOrEmpty(message)) message = "TestException.";
            if(string.IsNullOrEmpty(stackTrace)) stackTrace = "Стек вызовов не доступен.";
            ReasonText.Text = $"Конкретная причина остановки : {message}";
            textBox1.Text = stackTrace;
        }

        private void GoToGithubIssues(object sender, EventArgs e)
        {
            string url = "https://github.com/ZombiedEronix/ZPlayer/issues";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Program.PlayCrashSound();
                MessageBox.Show("Не удалось открыть браузер: " + ex.Message);
            }
        }

    }
}