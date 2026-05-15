namespace CrashHandler
{
    partial class ErrorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            ReasonText = new Label();
            githubHyper = new Label();
            Title = new Label();
            icon = new PictureBox();
            groupBox1 = new GroupBox();
            textBox1 = new TextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)icon).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ReasonText);
            panel1.Controls.Add(githubHyper);
            panel1.Controls.Add(Title);
            panel1.Controls.Add(icon);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(824, 129);
            panel1.TabIndex = 0;
            // 
            // ReasonText
            // 
            ReasonText.AutoSize = true;
            ReasonText.Location = new Point(101, 56);
            ReasonText.Name = "ReasonText";
            ReasonText.Size = new Size(251, 15);
            ReasonText.TabIndex = 3;
            ReasonText.Text = "Конкретная причина остановки : {Exception}";
            // 
            // githubHyper
            // 
            githubHyper.AutoSize = true;
            githubHyper.ForeColor = SystemColors.Highlight;
            githubHyper.Location = new Point(101, 94);
            githubHyper.Name = "githubHyper";
            githubHyper.Size = new Size(275, 15);
            githubHyper.TabIndex = 2;
            githubHyper.Text = "https://github.com/ZombiedEronix/ZPlayer/issues";
            githubHyper.Click += GoToGithubIssues;
            // 
            // Title
            // 
            Title.AutoSize = true;
            Title.Location = new Point(101, 26);
            Title.Name = "Title";
            Title.Size = new Size(256, 30);
            Title.TabIndex = 1;
            Title.Text = "При работе программы возникла проблема.\r\n\r\n";
            // 
            // icon
            // 
            icon.Anchor = AnchorStyles.Left;
            icon.Image = ZPlayerCrashHandler.Properties.Resources.soldier;
            icon.Location = new Point(12, 26);
            icon.Name = "icon";
            icon.Size = new Size(83, 83);
            icon.SizeMode = PictureBoxSizeMode.Zoom;
            icon.TabIndex = 0;
            icon.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlDark;
            groupBox1.Controls.Add(textBox1);
            groupBox1.ForeColor = SystemColors.ControlText;
            groupBox1.Location = new Point(12, 147);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(830, 284);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Стек вызовов";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.Control;
            textBox1.Location = new Point(6, 22);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(818, 256);
            textBox1.TabIndex = 0;
            // 
            // ErrorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(854, 445);
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ErrorForm";
            Text = "CrashHandler";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)icon).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private PictureBox icon;
        private Label Title;
        private Label githubHyper;
        private Label ReasonText;
        private GroupBox groupBox1;
        private TextBox textBox1;
    }
}