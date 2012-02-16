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

        protected static float[,] blockP = new float[blkSize, blkSize];
        protected static short[,] coefDctQnt = new short[blkSize, blkSize];
        protected static float[,] coefDct = new float[blkSize, blkSize];

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

                int numMcusX = (imgInfo.width + sizeBlockX - 1) / sizeBlockX;
                int numMcusY = (imgInfo.height + sizeBlockY - 1) / sizeBlockY;

                for (int mcu = 0; mcu < numMcusX * numMcusY; mcu = NextMcuPos(imgInfo, bReader, mcu))
                {
                    // X and Y coordinates of current MCU
                    int mcuX = mcu % numMcusX;
                    int mcuY = mcu / numMcusX;
                    // Starting X and Y pixels of current MCU
                    int ofsX = mcuX * sizeBlockX;
                    int ofsY = mcuY * sizeBlockY;

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

                    if (bReader.PastEndOfFile) break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.StackTrace);
            }

            Color2[,] imagen = MergeChannels(imgInfo, img);
            Bitmap bmp = new Bitmap(imgInfo.width, imgInfo.height);
            BmpData conv = new BmpData(bmp);

            bReader.StopReading();
            conv.SetImage(imagen);

            return bmp;
        }

        protected static int NextMcuPos(ImgInfo imgInfo, BitReader bReader, int mcu)
        {
            // If we are expecting a restart marker, find it in the stream,
            // reset the DC prediction variables and calculate the new MCU position
            // otherwise, just increment the position by one
            if (imgInfo.hasRestartMarkers &&
               (mcu % imgInfo.restartInterval) == imgInfo.restartInterval - 1)
            {
                Pixz.MarkersId currRestMarker = bReader.SyncStreamToNextRestartMarker();
                int difference = currRestMarker - imgInfo.prevRestMarker;

                if (difference <= 0) difference += Dri.RestartMarkerPeriod;

                ResetDeltas(imgInfo);
                imgInfo.mcuStrip += difference;
                imgInfo.prevRestMarker = currRestMarker;

                return imgInfo.mcuStrip * imgInfo.restartInterval;
            }
            else
            {
                return ++mcu;
            }
        }   

        protected static Color2[,] MergeChannels(ImgInfo imgInfo, float[][,] imgS)
        {
            Color2[,] img = new Color2[imgInfo.height, imgInfo.width];
            var converter = new Colorspaces.YCbCr();

            for (int y = 0; y < imgInfo.height; y++)
            {
                for (int x = 0; x < imgInfo.width; x++)
                {
                    Info info;

                    if (imgInfo.numOfComponents == 1) // Y
                    {
                        info.a = imgS[0][y, x];
                        info.b = 0;
                        info.c = 0;
                    }
                    else // YCbCr
                    {
                        info.a = imgS[0][y, x];
                        info.b = imgS[1][y, x];
                        info.c = imgS[2][y, x];
                    }

                    img[y, x] = converter.ConvertToRgb(info);
                }
            }

            return img;
        }

        protected static void DecodeBlock(BitReader bReader, ImgInfo imgInfo, float[,] img,
            int compIndex, int ofsX, int ofsY, int scaleX, int scaleY)
        {
            int quantIndex = imgInfo.components[compIndex].quantTableId;

            short[] coefZig = GetCoefficients(bReader, imgInfo, compIndex, blkSize * blkSize);
            FileOps.ZigZagToArray(coefZig, coefDctQnt, FileOps.tablasZigzag[blkSize], blkSize);
            ImgOps.Dequant(coefDctQnt, coefDct, imgInfo.quantTables[quantIndex].table, blkSize);
            ImgOps.Idct(coefDct, blockP, blkSize, blkSize);

            ImgOps.ResizeAndInsertBlock(imgInfo, blockP, img, blkSize, blkSize, ofsX, ofsY, scaleX, scaleY);
        }

        protected static short[] GetCoefficients(BitReader bReader, ImgInfo imgInfo, int compIndex, int numCoefs)
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
