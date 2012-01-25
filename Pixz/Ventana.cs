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
        Bitmap imgO;

        public Ventana()
        {
            InitializeComponent();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            //try
            //{
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    if (imgO != null) imgO.Dispose();

                    Stopwatch watch = new Stopwatch();

                    watch.Start();
                    if (Path.GetExtension(dialogo.FileName) == ".jpg")
                        imgO = Pixz.Decode(dialogo.FileName, (EncodeType)0);
                    else
                        imgO = new Bitmap(dialogo.FileName);

                    watch.Stop();
                    lblTiempo.Text = watch.ElapsedMilliseconds / 1000.0 + " s";

                    pbxOriginal.Image = imgO;
                    pbxOriginal.BackgroundImage = imgO;
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Ocurrió un error al abrir al archivo\n" + ex.Message);
            //}
        }
        

        private void ImprimirMatriz(ListBox lbxMatriz, float[,] matriz)
        {
            lbxMatriz.Items.Clear();

            for (int j = 0; j < matriz.GetLength(0); j++)
            {
                string renglon = string.Empty;

                for (int i = 0; i < matriz.GetLength(1); i++)
                {
                    renglon += String.Format("{0,3}", matriz[j,i])  + " ";
                }

                lbxMatriz.Items.Add(renglon);
            }
        }

        private void pbxOriginal_BackgroundImageChanged(object sender, EventArgs e)
        {
            pbxOriginal.Width = pbxOriginal.Image.Width;
            pbxOriginal.Height = pbxOriginal.Image.Height;
        }
    }
}
