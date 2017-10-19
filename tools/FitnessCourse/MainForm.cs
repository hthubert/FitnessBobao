using System;
using System.Windows.Forms;
using NAudio.Wave;

namespace Htggbb.FitnessCourse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var writer = new WaveFileWriter("test.wav", new WaveFormat());
            //writer.Write();
            //writer.w
        }
    }
}
