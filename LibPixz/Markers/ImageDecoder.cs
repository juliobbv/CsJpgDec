using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using LibPixz.Colorspaces;

namespace LibPixz.Markers
{
    public class ImageDecoder
    {
        const int blkSize = 8;

        protected internal static Bitmap DecodeImage(BinaryReader reader, ImgInfo imgInfo)
        {
            BitReader bReader = new BitReader(reader);
            float[][,] img = new float[imgInfo.numOfComponents][,];

            imgInfo.deltaDc = new short[imgInfo.numOfComponents];

            for (int ch = 0; ch < imgInfo.numOfComponents; ch++)
                img[ch] = new float[imgInfo.height, imgInfo.width];

            try
            {
                var componentMax = imgInfo.components.Aggregate((a, b) =>
                {
                    return new ComponentInfo()
                    {
                        samplingFactorX = Math.Max(a.samplingFactorX, b.samplingFactorX),
                        samplingFactorY = Math.Max(a.samplingFactorY, b.samplingFactorY)
                    };
                });

                int sizeBlockX = blkSize * componentMax.samplingFactorX;
                int sizeBlockY = blkSize * componentMax.samplingFactorY;

                int numTilesX = (imgInfo.width + sizeBlockX - 1) / sizeBlockX;
                int numTilesY = (imgInfo.height + sizeBlockY - 1) / sizeBlockY;

                for (int y = 0; y < numTilesY; y++)
                {
                    for (int x = 0; x < numTilesX; x++)
                    {
                        int ofsX = x * sizeBlockX;
                        int ofsY = y * sizeBlockY;

                        if (bReader.WasRestartMarkerFound()) { bReader.Flush(); ResetDeltas(imgInfo); }
                        if (bReader.PastEndOfFile) { break; }

                        for (int ch = 0; ch < imgInfo.numOfComponents; ch++)
                        {
                            for (int sy = 0; sy < imgInfo.components[ch].samplingFactorY; sy++)
                            {
                                for (int sx = 0; sx < imgInfo.components[ch].samplingFactorX; sx++)
                                {
                                    DecodeBlock(bReader, imgInfo, img[ch], ch, ofsX + blkSize * sx, ofsY + blkSize * sy,
                                        componentMax.samplingFactorX / imgInfo.components[ch].samplingFactorX,
                                        componentMax.samplingFactorY / imgInfo.components[ch].samplingFactorY);
                                }
                            }
                        }
                    }

                    if (bReader.PastEndOfFile) { break;  }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            Color[,] imagen = UnirCanales(imgInfo, img);
            Bitmap bmp = new Bitmap(imgInfo.width, imgInfo.height);
            BmpData conv = new BmpData(bmp);

            bReader.StopReading();

            conv.SetImage(imagen);

            return bmp;
        }

        protected static Color[,] UnirCanales(ImgInfo imgInfo, float[][,] imgS)
        {
            Color[,] img = new Color[imgInfo.height, imgInfo.width];
            var converter = new Colorspaces.YCbCr();

            for (int y = 0; y < imgInfo.height; y++)
            {
                for (int x = 0; x < imgInfo.width; x++)
                {
                    Info info;

                    info.a = imgS[0][y, x];
                    info.b = imgS[1][y, x];
                    info.c = imgS[2][y, x];

                    img[y, x] = converter.ConvertToRgb(info);
                }
            }

            return img;
        }

        protected static void DecodeBlock(BitReader bReader, ImgInfo imgInfo, float[,] img,
            int compIndex, int ofsX, int ofsY, int scaleX, int scaleY)
        {
            int quantIndex = imgInfo.components[compIndex].quantTableId;

            short[] coefZig = ObtenerCoef(bReader, imgInfo, compIndex, blkSize * blkSize);
            short[,] coefDctB = FileOps.ZigZagToArray(coefZig, FileOps.tablasZigzag[blkSize], blkSize);
            float[,] coefDct = ImgOps.Dequant(coefDctB, imgInfo.quantTables[quantIndex].table, blkSize);
            float[,] imgP = ImgOps.Dct(coefDct, blkSize, blkSize, true);

            if (scaleX != 1 || scaleY != 1)
                imgP = ImgOps.NearestNeighborResize(imgP, blkSize, blkSize, scaleX, scaleY);

            ComponerBloque(imgInfo, img, imgP, blkSize * scaleX, blkSize * scaleY, ofsX, ofsY);
        }

        protected static void ComponerBloque(ImgInfo imgInfo, float[,] imagen, float[,] bloque, int tamX, int tamY, int ofsX, int ofsY)
        {
            int finX = ofsX + tamX > imgInfo.width ? imgInfo.width % tamX : tamX;
            int finY = ofsY + tamY > imgInfo.height ? imgInfo.height % tamY : tamY;

            if (ofsX < imgInfo.width && ofsY < imgInfo.height)
            {
                for (int j = 0; j < finY; j++)
                {
                    for (int i = 0; i < finX; i++)
                    {
                        imagen[j + ofsY, i + ofsX] = bloque[j, i];
                    }
                }
            }
        }

        protected static short[] ObtenerCoef(BitReader bReader, ImgInfo imgInfo, int compIndex, int numCoefs)
        {
            var coefZig = new short[numCoefs];
            int acIndex = imgInfo.components[compIndex].acHuffmanTable;
            int dcIndex = imgInfo.components[compIndex].dcHuffmanTable;

            // DC coefficient
            uint runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[0, dcIndex]);

            uint run = runAmplitude >> 4;
            uint amplitude = runAmplitude & 0xf;

            coefZig[0] = (short)(Huffman.ReadCoefValue(bReader, amplitude) + imgInfo.deltaDc[compIndex]);

            imgInfo.deltaDc[compIndex] = coefZig[0];

            // AC coefficients
            uint pos = 0;

            while (pos < blkSize * blkSize - 1)
            {
                runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[1, acIndex]);

                if (runAmplitude == 0x00) break;

                run = runAmplitude >> 4;
                amplitude = runAmplitude & 0xf;
                pos += run;

                if (pos >= 63) break;

                coefZig[++pos] = Huffman.ReadCoefValue(bReader, amplitude);
            }

            return coefZig;
        }

        public static void ResetDeltas(ImgInfo imgInfo)
        {
            for (int i = 0; i < imgInfo.numOfComponents; i++)
            {
                imgInfo.deltaDc[i] = 0;
            }
        }
    }
}
