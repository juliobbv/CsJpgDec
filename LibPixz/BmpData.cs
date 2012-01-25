using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace LibPixz
{
    unsafe public class BmpData
    {
        private struct PixelData
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte alpha;

            public override string ToString()
            {
                return "(" + alpha.ToString() + ", " + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ")";
            }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public Bitmap WorkingBitmap
        {
            get { return workingBitmap; }
        }

        int width = 0;
        int height = 0;
        int stride = 0;
        Bitmap workingBitmap = null;
        BitmapData bitmapData = null;
        Byte* pBase = null;
        PixelData* pixelData = null;
        
        public BmpData(Bitmap inputBitmap)
        {
            workingBitmap = inputBitmap;
            width = this.workingBitmap.Width;
            height = this.workingBitmap.Height;
        }

        void LockImage()
        {
            Rectangle bounds = new Rectangle(Point.Empty, workingBitmap.Size);

            stride = (int)(bounds.Width * sizeof(PixelData));
            if (stride % 4 != 0) stride = 4 * (stride / 4 + 1);

            //Lock Image
            bitmapData = workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }

        public Color GetPixel(int x, int y)
        {
            pixelData = (PixelData*)(pBase + y * stride + x * sizeof(PixelData));
            return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* data = (PixelData*)(pBase + y * stride + x * sizeof(PixelData));
            data->alpha = color.A;
            data->red = color.R;
            data->green = color.G;
            data->blue = color.B;
        }

        public void UnlockImage()
        {
            workingBitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }

        public Color[,] GetImage()
        {
            Color[,] imagen = new Color[height, width];

            this.LockImage();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    imagen[y, x] = this.GetPixel(x, y);
                }
            }
            this.UnlockImage();

            return imagen;
        }

        public void SetImage(Color[,] imagen)
        {
            this.LockImage();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    this.SetPixel(x, y, imagen[y, x]);
                }
            }
            this.UnlockImage();
        }
    }
}
