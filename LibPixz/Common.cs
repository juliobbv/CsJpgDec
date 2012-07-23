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

        protected internal static float[,] Transpose(float[,] bloque, int tamX, int tamY)
        {
            float[,] blTrns = new float[tamX, tamY];

            for (int y = 0; y < tamY; y++)
                for (int x = 0; x < tamX; x++)
                    blTrns[x, y] = bloque[y, x];

            return blTrns;
        }

        protected internal static void PrintTable(Array array, int width, int height, int margin)
        {
            if (array.Rank != 1) return;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Logger.Write(String.Format("{0," + margin + "}", array.GetValue(j * height + i))  + " ");
                }

                Logger.WriteLine();
            }
        }

        protected internal static void PrintTable(Array array, int margin)
        {
            if (array.Rank != 2) return;

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Logger.Write(String.Format("{0," + margin + "}", array.GetValue(j,  i)) + " ");
                }

                Logger.WriteLine();
            }
        }

        protected internal static void PrintHuffmanTable(List<Huffman.CodeInfo> table)
        {
            Logger.WriteLine("       Code        Number     Length");

            foreach (var entry in table)
            {
                Logger.Write(FormatString(ToBinary(entry), 16));
                Logger.Write(FormatString(entry.number, 8));
                Logger.WriteLine(FormatString(entry.length, 8));
            }
        }

        protected internal static string FormatString(string label, object value, int margin)
        {
            return label + ": " + String.Format("{0," + margin + "}", value.ToString()) + " ";
        }

        protected internal static string FormatString(object value, int margin)
        {
            return String.Format("{0," + margin + "}", value.ToString()) + " ";
        }

        static string ToBinary(Huffman.CodeInfo number)
        {
            string numStr = string.Empty;

            while (number.length > 0)
            {
                numStr = (number.code & 1) + numStr;
                number.code >>= 1;
                number.length--;
            }

            return numStr;
        }

    }
}
