using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LibPixz.Colorspaces
{
    public class YCbCr : IColorspaceConverter
    {
        protected static float[,] mRgbYcbcr = 
        {
            {  0.299f,   0.587f,   0.114f  },
            { -0.1687f, -0.3313f,  0.5f    },
            {  0.5f,    -0.4187f, -0.0813f }
        };

        protected static float[,] mYcbcrRgb = 
        {
            {  1f,  0f,        1.402f   },
            {  1f, -0.34414f, -0.71414f },
            {  1f,  1.772f,    0f       }
        };

        public Color ConvertToRgb(Info info)
        {
            byte r, g, b;
            float y  = info.a + 128;
            float cb = info.b;
            float cr = info.c;

            r = (byte)Common.Clamp(y                        + cr * mYcbcrRgb[0, 2], 0, 255);
            g = (byte)Common.Clamp(y + cb * mYcbcrRgb[1, 1] + cr * mYcbcrRgb[1, 2], 0, 255);
            b = (byte)Common.Clamp(y + cb * mYcbcrRgb[2, 1]                       , 0, 255);

            return Color.FromArgb(r, g, b);
        }

        public Info ConvertFromRgb(System.Drawing.Color rgb)
        {
            Info ycbcr;

            // Valores YCbCr
            ycbcr.a = rgb.R * mRgbYcbcr[0, 0] + rgb.G * mRgbYcbcr[0, 1] + rgb.B * mRgbYcbcr[0, 2] - 128;
            ycbcr.b = rgb.R * mRgbYcbcr[1, 0] + rgb.G * mRgbYcbcr[1, 1] + rgb.B * mRgbYcbcr[1, 2];
            ycbcr.c = rgb.R * mRgbYcbcr[2, 0] + rgb.G * mRgbYcbcr[2, 1] + rgb.B * mRgbYcbcr[2, 2];

            //if (ycbcr.a > 255f || ycbcr.b > 255f || ycbcr.c > 255f)
            //    Console.WriteLine("Valor ycbcr desbordado");

            return ycbcr;
        }
    }
}
