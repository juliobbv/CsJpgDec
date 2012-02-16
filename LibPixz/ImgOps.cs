using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;

namespace LibPixz
{
    unsafe public partial class ImgOps
    {
        public static Dictionary<int, float[,]> tablaCosActual;

        public static Dictionary<int, float[,]> tablasCos = new Dictionary<int, float[,]>()
        {
            { 1, GetTablaCos(1) },
            { 2, GetTablaCos(2) },
            { 4, GetTablaCos(4) },
            { 8, GetTablaCos(8) },
            { 16, GetTablaCos(16) },
            { 32, GetTablaCos(32) },
            { 64, GetTablaCos(64) },
        };

        public static Dictionary<int, float[,]> tablasICos = new Dictionary<int, float[,]>()
        {
            { 1, GetTablaICos(1) },
            { 2, GetTablaICos(2) },
            { 4, GetTablaICos(4) },
            { 8, GetTablaICos(8) },
            { 16, GetTablaICos(16) },
            { 32, GetTablaICos(32) },
            { 64, GetTablaICos(64) },
        };

        /*static Dictionary<int, int> bitsPorNum = new Dictionary<int, int>()
        {
            {1, 0}, {2, 1}, {4, 2}, {8, 3}, {16, 4}, {32, 5}, {64, 6}
        };*/

        static int[] bitsPorNum = { 0, 0, 1, 0, 2, 0, 0, 0,
                                    3, 0, 0, 0, 0, 0, 0, 0, 
                                    4, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0,
                                    5, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0,
                                    6
                                  };

        protected static float[,] GetTablaCos(int tam)
        {
            float[,] tablaCosXU = new float[tam, tam];

            for (int u = 0; u < tam; u++)
            {
                for (int x = 0; x < tam; x++)
                {
                    tablaCosXU[x, u] = (float)Math.Cos(((2 * x + 1) * u * Math.PI) / (2 * tam));
                    tablaCosXU[x, u] *= (float)Math.Sqrt(2.0 / tam);
                    if (u == 0) tablaCosXU[x, u] /= (float)Math.Sqrt(2f);
                }
            }

            return tablaCosXU;
        }

        protected static float[,] GetTablaICos(int tam)
        {
            float[,] tablaCosUX = new float[tam, tam];

            for (int u = 0; u < tam; u++)
            {
                for (int x = 0; x < tam; x++)
                {
                    tablaCosUX[u, x] = (float)Math.Cos(((2 * x + 1) * u * Math.PI) / (2 * tam));
                    tablaCosUX[u, x] *= (float)Math.Sqrt(2.0 / tam);
                    if (u == 0) tablaCosUX[u, x] /= (float)Math.Sqrt(2f);
                }
            }

            return tablaCosUX;
        }

        static float[] Fct(float* bloque, int tam, float[,] tCos)
        {
            int numNiveles = bitsPorNum[tam];
            float[] coefImpar = new float[tam];
            float[] coefPar = new float[tam];
            float[] res = new float[tam];

            for (int i = 0; i < tam; i++)
                coefPar[i] = bloque[i];

            int tamAct = tam;

            // Se calculan los coeficientes AC, basándose en qué nivel pertenecen
            // Se crean niveles, estos dependen de qué cuánta factorización es posible para cada coeficiente
            // mientras más alto el nivel, más pixeles se pueden agrupar, y así reducir el no. de operaciones
            for (int nivel = 0; nivel <= numNiveles; nivel++)
            {
                int iu = 1 << nivel;
                int step = iu << 1;
                int hTam = tamAct / 2;

                for (int i = 0; i < hTam; i++)
                {
                    Common.Butterfly(coefPar[i], coefPar[tamAct - i - 1], ref coefPar[i], ref coefImpar[i]);
                }

                for (int u = iu; u < tam; u += step)
                {
                    for (int x = 0; x < hTam; x++)
                    {
                        res[u] += coefImpar[x] * tCos[x, u];
                    }
                }

                tamAct /= 2;
            }

            // Si es DC, entonces es el último nivel, y nunca se presenta simetría impar,
            // así que el obtenemos el único valor de ese nivel
            res[0] = coefPar[0] * tCos[0, 0];

            return res;
        }

        static float[] Ifct(float* bloque, int tam, float[,] tIcos)
        {
            int numNiveles = bitsPorNum[tam];
            float[] res = new float[tam];

            //int tamAct = tam;

            // Se calculan los coeficientes AC, basándose en qué nivel pertenecen
            // Se crean niveles, estos dependen de qué cuánta factorización es posible para cada coeficiente
            // mientras más alto el nivel, más pixeles se pueden agrupar, y así reducir el no. de operaciones
            for (int nivel = 0; nivel <= numNiveles; nivel++)
            {
                int iu = 1 << nivel;
                int step = iu << 1;
                int tamAct = tam >> nivel;

                for (int x = 0; x < tamAct / 2; x++)
                {
                    float suma = 0;

                    for (int u = iu; u < tam; u += step)
                    {
                        suma += bloque[u] * tIcos[u, x];
                    }

                    for (int i = 0; i < iu; i += 2)
                    {
                        res[i * tamAct + x] += suma;
                        res[i * tamAct + tamAct - x - 1] -= suma;
                    }

                    for (int i = 1; i < iu; i += 2)
                    {
                        res[i * tamAct + x] -= suma;
                        res[i * tamAct + tamAct - x - 1] += suma;
                    }
                }
            }

            for (int x = 0; x < tam; x++)
            {
                res[x] += bloque[0] * tIcos[0, 0];
            }

            return res;
        }

        protected internal static float[,] Fdct(float[,] bloque, int tamX, int tamY)
        {
            float[,] bloqueDct = new float[tamY, tamX];
            float[][] coefYU = new float[tamY][];
            float[,] coefUY = new float[tamX, tamY];
            float[][] coefUV = new float[tamY][];

            float[,] tCosXU = tablasCos[tamX];
            float[,] tCosYV = tablasCos[tamY];

            // Sacamos el DCT de cada fila del bloque
            fixed (float* inicio = bloque)
            {
                float* renglon = inicio;
                //Parallel.For(0, tamY, y =>
                for (int y = 0; y < tamY; y++)
                {
                    coefYU[y] = Fct(y * tamX + renglon, tamX, tCosXU);
                }//);
            }

            coefUY = Common.Transpose(coefYU, tamX, tamY);

            // Ahora sacamos el DCT por columna de los resultados anteriores
            fixed (float* inicio = coefUY)
            {
                float* columna = inicio;
                for (int u = 0; u < tamX; u++)
                //Parallel.For(0, tamX, u =>
                {
                    coefUV[u] = Fct(u * tamY + columna, tamY, tCosYV);
                }//);
            }

            for (int v = 0; v < tamY; v++)
                for (int u = 0; u < tamX; u++)
                    bloqueDct[v, u] = (float)Math.Round(coefUV[u][v]);

            return bloqueDct;
        }

        protected internal static float[,] Fidct(float[,] bloque, int tamX, int tamY)
        {
            float[,] bloqueDct = new float[tamY, tamX];
            float[][] coefYU = new float[tamY][];
            float[,] coefUY = new float[tamX, tamY];
            float[][] coefUV = new float[tamY][];

            float[,] tCosXU = tablasICos[tamX];
            float[,] tCosYV = tablasICos[tamY];

            // Sacamos el IDCT de cada fila del bloque
            fixed (float* inicio = bloque)
            {
                float* renglon = inicio;
                //Parallel.For(0, tamY, y =>
                for (int y = 0; y < tamY; y++)
                {
                    coefYU[y] = Ifct(y * tamX + renglon, tamX, tCosXU);
                }//);
            }

            coefUY = Common.Transpose(coefYU, tamX, tamY);

            // Ahora sacamos el DCT por columna de los resultados anteriores
            fixed (float* inicio = coefUY)
            {
                float* columna = inicio;
                for (int u = 0; u < tamX; u++)
                //Parallel.For(0, tamX, u =>
                {
                    coefUV[u] = Ifct(u * tamY + columna, tamY, tCosYV);
                }//);
            }

            for (int v = 0; v < tamY; v++)
                for (int u = 0; u < tamX; u++)
                    bloqueDct[v, u] = (float)Math.Round(coefUV[u][v]);

            return bloqueDct;
        }

        protected internal static float[,] Dct(float[,] bloque, int tamX, int tamY, bool inversa = false)
        {
            int u, v, x, y;
            float suma, suma2;
            float[,] bloqueDct = new float[tamY, tamX];

            float[,] tCosXU = inversa ? tablasICos[tamX] : tablasCos[tamX];
            float[,] tCosYV = inversa ? tablasICos[tamY] : tablasCos[tamY];

            for (v = 0; v < tamY; v++)
            {
                for (u = 0; u < tamX; u++)
                {
                    for (y = 0, suma2 = 0; y < tamY; y++)
                    {
                        for (x = 0, suma = 0; x < tamX; x++)
                        {
                            suma += (bloque[y, x]) * tCosXU[x, u];
                        }

                        suma2 += suma * tCosYV[y, v];
                    }
                    bloqueDct[v, u] = (float)(Math.Round(suma2));
                }
            }

            return bloqueDct;
        }

        protected internal static float[,] DctP(float[,] bloque, int tamX, int tamY, bool inversa = false)
        {
            float[,] bloqueDct = new float[tamY, tamX];

            float[,] tCosXU = inversa ? tablasICos[tamX] : tablasCos[tamX];
            float[,] tCosYV = inversa ? tablasICos[tamY] : tablasCos[tamY];

            Parallel.For(0, tamY, v =>
            {
                for (int u = 0; u < tamX; u++)
                {
                    float suma2 = 0;
                    for (int y = 0; y < tamY; y++)
                    {
                        float suma = 0;
                        for (int x = 0; x < tamX; x++)
                        {
                            suma += bloque[y, x] * tCosXU[x, u];
                        }

                        suma2 += suma * tCosYV[y, v];
                    }
                    bloqueDct[v, u] = (float)(Math.Round(suma2));
                }
            });

            return bloqueDct;
        }

        protected internal static void MostrarBordes(float[,] coefDct, int tam)
        {
            for (int y = 0; y < tam; y++)
                coefDct[y, tam - 1] = 96f;

            for (int x = 0; x < tam; x++)
                coefDct[tam - 1, x] = 96f;
        }

        protected internal static void ResizeAndInsertBlock(ImgInfo imgInfo, float[,] block, float[,] imagen, int tamX, int tamY, int ofsX, int ofsY, int scaleX, int scaleY)
        {
            // Nearest neighbor interpolation

            // For staircase FTW
            if (ofsX < imgInfo.width && ofsY < imgInfo.height)
            {
                for (int j = 0; j < tamY; j++)
                {
                    for (int i = 0; i < tamX; i++)
                    {
                        for (int jj = 0; jj < scaleY; jj++)
                        {
                            for (int ii = 0; ii < scaleX; ii++)
                            {
                                int posYimg = j * scaleY + ofsY + jj;
                                int posXimg = i * scaleX + ofsX + ii;

                                if (posYimg < imgInfo.height && posXimg < imgInfo.width)
                                {
                                    imagen[posYimg, posXimg] = block[j, i];
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
