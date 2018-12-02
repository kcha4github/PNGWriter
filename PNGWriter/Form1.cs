using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PNGEncoder;


namespace PNGWriter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bmp;

        private void OpenImageFile(string path)
        {
            try
            {
                bmp = new Bitmap(path);
                saveToolStripMenuItem.Enabled = true;
                pictureBox1.Invalidate();
            }
            catch (Exception)
            {
            }
        }

        private void SavePNGFile(string path)
        {
            if (null == this.bmp)
                return;

            PNGEncoder.Encoder encoder = new PNGEncoder.Encoder(this.bmp);
            encoder.Encode();
            encoder.SavePNG(path);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != openFileDialog1.ShowDialog())
                return;

            string filename = openFileDialog1.FileName;
            OpenImageFile(filename);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (null == this.bmp)
                return;

            e.Graphics.DrawImage(bmp, 0, 0);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != saveFileDialog1.ShowDialog())
                return;

            string filename = saveFileDialog1.FileName;
            SavePNGFile(filename);
        }
    }
}