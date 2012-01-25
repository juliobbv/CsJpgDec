using System;
using System.Collections.Generic;
using System.IO;
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
            short[] deltaDc = new short[3];

            for (int ch = 0; ch < imgInfo.numOfComponents; ch++)
                img[ch] = new float[imgInfo.height, imgInfo.width];

            try
            {
                // My daily WTF
                if (imgInfo.components[0].samplingFactorX == 2 &&
                    imgInfo.components[0].samplingFactorY == 2 &&
                    imgInfo.components[1].samplingFactorX == 1 &&
                    imgInfo.components[1].samplingFactorY == 1 &&
                    imgInfo.components[2].samplingFactorX == 1 &&
                    imgInfo.components[2].samplingFactorY == 1)
                {
                    int numTilesX = (imgInfo.width + blkSize * 2 - 1) / (blkSize * 2);
                    int numTilesY = (imgInfo.height + blkSize * 2 - 1) / (blkSize * 2);

                    for (int y = 0; y < numTilesY; y++)
                    {
                        for (int x = 0; x < numTilesX; x++)
                        {
                            int ofsX = x * blkSize * 2;
                            int ofsY = y * blkSize * 2;

                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY, ref deltaDc[0], 1, 1); // Y0
                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX + blkSize, ofsY, ref deltaDc[0], 1, 1); // Y1
                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY + blkSize, ref deltaDc[0], 1, 1); // Y2
                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX + blkSize, ofsY + blkSize, ref deltaDc[0], 1, 1); // Y3
                            DecodeBlock(bReader, imgInfo, img[1], 1, ofsX, ofsY, ref deltaDc[1], 2, 2); // Cb
                            DecodeBlock(bReader, imgInfo, img[2], 2, ofsX, ofsY, ref deltaDc[2], 2, 2); // Cr
                        }
                    }
                }
                else if (imgInfo.components[0].samplingFactorX == 2 &&
                         imgInfo.components[0].samplingFactorY == 1 &&
                         imgInfo.components[1].samplingFactorX == 1 &&
                         imgInfo.components[1].samplingFactorY == 1 &&
                         imgInfo.components[2].samplingFactorX == 1 &&
                         imgInfo.components[2].samplingFactorY == 1)
                {
                    int numTilesX = (imgInfo.width + blkSize * 2 - 1) / (blkSize * 2);
                    int numTilesY = (imgInfo.height + blkSize - 1) / blkSize;

                    for (int y = 0; y < numTilesY; y++)
                    {
                        for (int x = 0; x < numTilesX / 2; x++)
                        {
                            int ofsX = x * blkSize * 2;
                            int ofsY = y * blkSize;

                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY, ref deltaDc[0], 1, 1); // Y0
                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX + blkSize, ofsY, ref deltaDc[0], 1, 1); // Y1
                            DecodeBlock(bReader, imgInfo, img[1], 1, ofsX, ofsY, ref deltaDc[1], 2, 1); // Cb
                            DecodeBlock(bReader, imgInfo, img[2], 2, ofsX, ofsY, ref deltaDc[2], 2, 1); // Cr
                        }
                    }
                }
                else if (imgInfo.components[0].samplingFactorX == 1 &&
                         imgInfo.components[0].samplingFactorY == 2 &&
                         imgInfo.components[1].samplingFactorX == 1 &&
                         imgInfo.components[1].samplingFactorY == 1 &&
                         imgInfo.components[2].samplingFactorX == 1 &&
                         imgInfo.components[2].samplingFactorY == 1)
                {
                    int numTilesX = (imgInfo.width + blkSize - 1) / blkSize;
                    int numTilesY = (imgInfo.height + blkSize * 2 - 1) / (blkSize * 2);

                    for (int y = 0; y < numTilesY; y++)
                    {
                        for (int x = 0; x < numTilesX; x++)
                        {
                            int ofsX = x * blkSize;
                            int ofsY = y * blkSize * 2;

                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY, ref deltaDc[0], 1, 1); // Y0
                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY + blkSize, ref deltaDc[0], 1, 1); // Y1
                            DecodeBlock(bReader, imgInfo, img[1], 1, ofsX, ofsY, ref deltaDc[1], 1, 2); // Cb
                            DecodeBlock(bReader, imgInfo, img[2], 2, ofsX, ofsY, ref deltaDc[2], 1, 2); // Cr
                        }
                    }
                }
                else if (imgInfo.components[0].samplingFactorX == 1 &&
                         imgInfo.components[0].samplingFactorY == 1 &&
                         imgInfo.components[1].samplingFactorX == 1 &&
                         imgInfo.components[1].samplingFactorY == 1 &&
                         imgInfo.components[2].samplingFactorX == 1 &&
                         imgInfo.components[2].samplingFactorY == 1)
                {
                    int numTilesX = (imgInfo.width + blkSize - 1) / blkSize;
                    int numTilesY = (imgInfo.height + blkSize - 1) / blkSize;

                    for (int y = 0; y < numTilesY; y++)
                    {
                        for (int x = 0; x < numTilesX; x++)
                        {
                            int ofsX = x * blkSize;
                            int ofsY = y * blkSize;

                            DecodeBlock(bReader, imgInfo, img[0], 0, ofsX, ofsY, ref deltaDc[0], 1, 1); // Y0
                            DecodeBlock(bReader, imgInfo, img[1], 1, ofsX, ofsY, ref deltaDc[1], 1, 1); // Cb
                            DecodeBlock(bReader, imgInfo, img[2], 2, ofsX, ofsY, ref deltaDc[2], 1, 1); // Cr
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception) { }

            Color[,] imagen = UnirCanales(imgInfo, img);
            Bitmap bmp = new Bitmap(imgInfo.width, imgInfo.height);
            BmpData conv = new BmpData(bmp);

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
            int compIndex, int ofsX, int ofsY, ref short deltaDc, int scaleX, int scaleY)
        {
            int quantIndex = imgInfo.components[compIndex].quantTableId;

            short[] coefZig = ObtenerCoef(bReader, imgInfo, compIndex, blkSize * blkSize, ref deltaDc);
            short[,] coefDctB = FileOps.ComponerZigZag(coefZig, FileOps.tablasZigzag[blkSize], blkSize);
            float[,] coefDct = ImgOps.Dequant(coefDctB, imgInfo.quantTables[quantIndex].table, blkSize);
            float[,] imgP = ImgOps.Dct(coefDct, blkSize, blkSize, true);

            if (scaleX != 1 && scaleY != 1)
                imgP = ImgOps.BilinearResize(imgP, blkSize, blkSize, scaleX, scaleY);

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

        protected static short[] ObtenerCoef(BitReader bReader, ImgInfo imgInfo, int compIndex, int numCoefs, ref short deltaDc)
        {
            var coefZig = new short[numCoefs];
            int acIndex = imgInfo.components[compIndex].acHuffmanTable;
            int dcIndex = imgInfo.components[compIndex].dcHuffmanTable;

            // DC coefficient
            uint runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[0, dcIndex]);
            uint run = runAmplitude >> 4;
            uint amplitude = runAmplitude & 0xf;

            coefZig[0] = (short)(Huffman.ReadCoefValue(bReader, amplitude) + deltaDc);

            deltaDc = coefZig[0];

            // AC coefficients
            uint pos = 0;

            while (pos < blkSize * blkSize - 1)
            {
                runAmplitude = Huffman.ReadRunAmplitude(bReader, imgInfo.huffmanTables[1, acIndex]);

                if (runAmplitude == 0x00) break;

                run = runAmplitude >> 4;
                amplitude = runAmplitude & 0xf;
                pos += run;

                if (pos > 63) break;

                coefZig[++pos] = Huffman.ReadCoefValue(bReader, amplitude);
            }

            return coefZig;
        }
    }
}
