using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibPixz
{
    public partial class ImgOps
    {
        protected internal static short[,] Quant(float[,] pixDct, ushort[] matriz, int tam)
        {
            short[,] pixQnt = new short[tam, tam];

            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    float val = (float)Math.Round(pixDct[y, x] / matriz[y * tam + x]);

                    pixQnt[y, x] = (short)Common.Clamp(val, short.MinValue, short.MaxValue);
                }
            }

            return pixQnt;
        }

        protected internal static float[,] Dequant(short[,] pixQnt, ushort[] matriz, int tam)
        {
            float[,] pixDct = new float[tam, tam];

            for (int y = 0; y < tam; y++)
            {
                for (int x = 0; x < tam; x++)
                {
                    pixDct[y, x] = (float)(pixQnt[y, x] * matriz[y * tam + x]);
                }
            }

            return pixDct;
        }
    }
}
