using System;
using System.Windows.Forms;
using NAudio.Wave;

namespace Htggbb.FitnessCourse
{
    public partial class FormMain : Form
    {
        private SpeechCreator _creator;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnSynthesis_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK) {
                var task = _creator.Process(openDialog.FileName);
                while (!task.Wait(0)) {
                    Application.DoEvents();
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _creator = new SpeechCreator();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            var task = _creator.Init();
            while (!task.Wait(0)) {
                Application.DoEvents();
            }
        }

        private void btnTTS_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContent.Text)) {
                return;
            }
            if (saveDialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            var task = _creator.Process(txtContent.Lines, saveDialog.FileName);
            while (!task.Wait(0)) {
                Application.DoEvents();
            }
        }
    }
}
