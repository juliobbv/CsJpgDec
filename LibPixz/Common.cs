using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibPixz
{
    public enum ColorType
    {
        Lab,
        YCbCr,
        Otro
    }

    public enum EncodeType
    {
        Normal,
        Multithreaded,
        Fast
    }

    class Common
    {
        protected internal static float Clamp(float num, float min, float max)
        {
            if (num < min)
                return min;
            else if (num > max)
                return max;

            return num;
        }

        protected internal static void Butterfly(float a, float b, ref float c, ref float d)
        {
            c = a + b;
            d = a - b;
        }

        protected internal static float[,] Transpose(float[][] bloque, int tamX, int tamY)
        {
            float[,] blTrns = new float[tamX, tamY];

            for (int y = 0; y < tamY; y++)
                for (int x = 0; x < tamX; x++)
                    blTrns[x, y] = bloque[y][x];

            return blTrns;
        }
    }
}
