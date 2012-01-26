using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    public class BitReader
    {
        const uint dataSize = sizeof(ushort) * 8;
        const uint readerSize = sizeof(byte) * 8;
        BinaryReader reader;
        uint readData;
        uint availableBits;
        bool dataPad;

        /// <summary>
        /// How many bits we read at once in the stream
        /// </summary>
        public uint BitStride
        {
            get { return dataSize; }
        }

        public BitReader(BinaryReader reader)
        {
            availableBits = 0;
            readData = 0;
            dataPad = false;

            this.reader = reader;
        }

        public ushort Peek(uint length, out byte restartMarker)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            // If we don't have as many bits as needed, read another chunk from stream
            if (length > availableBits)
            {
                byte nextChunk = 0;

                try
                {
                    while (availableBits <= length)
                    {
                        nextChunk = ReadByteNonStuffed(out restartMarker);

                        if (restartMarker != 0x00) return 0;

                        availableBits += readerSize;
                        readData = (readData << (int)readerSize) | nextChunk;
                    }
                }
                // If already at the end of stream, next chunk will be all zeros,
                // so we can decode the last blocks of the image
                catch (Exception)
                {
                    if (dataPad)
                        throw new Exception("Reading two padding chunks, stream may be faulty");

                    dataPad = true;
                }
            }

            // We move data left and right in order to get only the bits we require
            uint cleanData = readData << (int)(dataSize * 2 - availableBits);
            cleanData >>= (int)(dataSize * 2 - length);
            restartMarker = 0;

            return (ushort)cleanData;
        }

        public ushort Read(uint length, out byte restartMarker)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            ushort data = Peek(length, out restartMarker);

            if (restartMarker != 0)
            {
                availableBits = 0;
                data = 0;

                return 0;
            }

            availableBits -= length;

            int shift = (int)(dataSize * 2 - availableBits);
            // We move data left and right in order to get only the bits we require
            readData <<= shift;
            readData >>= shift;

            return data;
        }

        public void StopReading()
        {
            // Rewind all those bytes we didn't use
            uint rewind = availableBits / sizeof(byte);

            reader.BaseStream.Seek(-rewind, SeekOrigin.Current);
            availableBits = 0;
            readData = 0;
        }

        public BinaryReader GetBinaryReader()
        {
            return reader;
        }

        public byte ReadByteNonStuffed(out byte restartMarker)
        {
            byte number = reader.ReadByte();

            restartMarker = 0;

            if (number == 0xff)
            {
                byte markerValue = reader.ReadByte();

                if (markerValue == 0x00)
                {
                    return number;
                }
                else if (markerValue >= 0xd0 && markerValue <= 0xd7)
                {
                    restartMarker = markerValue;
                    return 0;
                }
                else
                {
                    restartMarker = 0;
                    return reader.ReadByte();
                }
            }
            else
            {
                return number;
            }
        }
    }
}
