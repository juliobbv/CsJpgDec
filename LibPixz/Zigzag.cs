using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LibPixz
{
    public class FileOps
    {
        protected internal static Dictionary<int, Point[]> tablasZigzag = new Dictionary<int, Point[]>()
        {
            { 8, FileOps.GetTablaZigzag(8, 8) },
            { 16, FileOps.GetTablaZigzag(16, 16) },
            { 32, FileOps.GetTablaZigzag(32, 32) },
            { 64, FileOps.GetTablaZigzag(64, 64) }
        };

        protected internal static Point[] GetTablaZigzag(int tamX, int tamY)
        {
            if (tamX <= 0 || tamY <= 0)
                throw new Exception("Las dimensiones del bloque no pueden ser 0 ni negativas");

            Point[] tabla = new Point[tamY * tamX];
            int x = 0, y = 0;
            int pos = 0;

            tabla[pos++] = new Point(x, y);

            while (pos < tamY * tamX)
            {
                if (x == tamX - 1)
                    tabla[pos++] = new Point(x, ++y);
                else
                    tabla[pos++] = new Point(++x, y);

                if (pos == tamY * tamX) break;

                while (x > 0 && y < tamY - 1)
                    tabla[pos++] = new Point(--x, ++y);

                if (y == tamY - 1)
                    tabla[pos++] = new Point(++x, y);
                else
                    tabla[pos++] = new Point(x, ++y);

                if (pos == tamY * tamX) break;

                while (y > 0 && x < tamX - 1)
                    tabla[pos++] = new Point(++x, --y);
            }

            return tabla;
        }

        protected internal static short[] ReordenarZigZag(short[,] coefDct, Point[] orden, int tam)
        {
            int numElem = tam * tam;
            short[] coefZig = new short[numElem];

            // Dejamos un espacio para los parámetros del cuadro
            for (int i = 0; i < numElem; i++)
            {
                coefZig[i + 1] = coefDct[orden[i].Y, orden[i].X];
            }

            return coefZig;
        }

        protected internal static short[,] ComponerZigZag(short[] coefZig, Point[] orden, int tam)
        {
            int numElem = tam * tam;
            short[,] coefDct = new short[tam, tam];

            for (int i = 0; i < numElem; i++)
            {
                coefDct[orden[i].Y, orden[i].X] = coefZig[i];
            }

            return coefDct;
        }
    }
}