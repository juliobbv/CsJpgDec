using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using LibPixz;

namespace PixzGui
{
    public partial class Ventana : Form
    {
        List<Bitmap> images;

        public Ventana()
        {
            InitializeComponent();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Filter = "JPEG Images (*.jpg; *.jpeg;)|*.jpg; *.jpeg; | All files (*.*)|*.*";

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    Stopwatch watch = new Stopwatch();

                    watch.Start();
                    images = Pixz.Decode(dialogo.FileName);
                    watch.Stop();

                    lblTiempo.Text = watch.ElapsedMilliseconds / 1000.0 + " s";

                    if (images.Count == 0)
                    {
                        MessageBox.Show("No se encontraron imágenes en el archivo");
                        return;
                    }

                    pbxOriginal.Image = images[0];

                    nudImagen.Value = 0;
                    nudImagen.Minimum = 0;
                    nudImagen.Maximum = images.Count - 1;
                }
            }
        }

        private void pbxOriginal_BackgroundImageChanged(object sender, EventArgs e)
        {
            pbxOriginal.Width = pbxOriginal.Image.Width;
            pbxOriginal.Height = pbxOriginal.Image.Height;
        }

        private void nudImagen_ValueChanged(object sender, EventArgs e)
        {
            if (images == null) return;

            int value = (int)(sender as NumericUpDown).Value;

            pbxOriginal.Image = images[value];
        }
    }
}
