using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using LibPixz.Colorspaces;

namespace LibPixz
{
    unsafe internal class BmpData
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

        internal int Width
        {
            get { return width; }
        }

        internal int Height
        {
            get { return height; }
        }

        internal Bitmap WorkingBitmap
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
        
        internal BmpData(Bitmap inputBitmap)
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

        internal Color2 GetPixel(int x, int y)
        {
            pixelData = (PixelData*)(pBase + y * stride + x * sizeof(PixelData));
            return new Color2() { a = pixelData->alpha, r = pixelData->red, g = pixelData->green, b = pixelData->blue };
        }

        internal void SetPixel(int x, int y, Color2 color)
        {
            PixelData* data = (PixelData*)(pBase + y * stride + x * sizeof(PixelData));
            data->alpha = color.a;
            data->red = color.r;
            data->green = color.g;
            data->blue = color.b;
        }

        internal void UnlockImage()
        {
            workingBitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }

        internal Color2[,] GetImage()
        {
            Color2[,] imagen = new Color2[height, width];

            this.LockImage();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    imagen[y, x] = this.GetPixel(x, y);
                }
            }
            this.UnlockImage();

            return imagen;
        }

        internal void SetImage(Color2[,] imagen)
        {
            this.LockImage();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SetPixel(x, y, imagen[y, x]);
                }
            }
            this.UnlockImage();
        }
    }
}
