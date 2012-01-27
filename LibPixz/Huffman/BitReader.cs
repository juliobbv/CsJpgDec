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
        uint nonFakeBits;
        bool dataPad;
        byte markerData;

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
            nonFakeBits = 0;
            readData = 0;
            markerData = 0;
            dataPad = false;

            this.reader = reader;
        }

        public ushort Peek(uint length)
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
                        nextChunk = ReadByteNonStuffed();
                        availableBits += readerSize;
                        readData = (readData << (int)readerSize) | nextChunk;

                        if (markerData == 0x00) nonFakeBits = availableBits;
                    }
                }
                // If already at the end of stream, next chunk will be all zeros,
                // so we can decode the last blocks of the image
                catch (Exception ex)
                {
                    if (ex.Message == "Restart") throw;

                    if (dataPad)
                        throw new Exception("Reading two padding chunks, stream may be faulty");

                    dataPad = true;
                }
            }

            // We move data left and right in order to get only the bits we require
            uint cleanData = readData << (int)(dataSize * 2 - availableBits);
            cleanData >>= (int)(dataSize * 2 - length);

            return (ushort)cleanData;
        }

        public ushort Read(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            if ((markerData != 0x00) && (length > nonFakeBits))
            {
                PurgeData();
                throw new Exception("Restart");
            }

            ushort data = Peek(length);

            availableBits -= length;
            nonFakeBits -= length;

            int shift = (int)(dataSize * 2 - availableBits);
            // We move data left and right in order to get only the bits we require
            readData <<= shift;
            readData >>= shift;

            return data;
        }

        public void StopReading()
        {
            // Rewind all those bytes we didn't use
            uint rewind = nonFakeBits / sizeof(byte);

            reader.BaseStream.Seek(-rewind, SeekOrigin.Current);
            PurgeData();
        }

        public BinaryReader GetBinaryReader()
        {
            return reader;
        }

        public byte ReadByteNonStuffed()
        {
            if (markerData != 0x00) return 0;

            byte number = reader.ReadByte();

            if (number == 0xff)
            {
                byte markerValue = reader.ReadByte();

                if (markerValue == 0x00)
                {
                    return number;
                }
                else if (markerValue >= 0xd0 && markerValue <= 0xd7)
                {
                    markerData = markerValue;
                    return 0;
                }
                else
                {
                    return reader.ReadByte();
                }
            }
            else
            {
                return number;
            }
        }

        public bool WasRestartMarkerFound()
        {
            if (markerData != 0)
            {
                PurgeData();
                return true;
            }

            return false;
        }

        public void PurgeData()
        {
            nonFakeBits = 0;
            availableBits = 0;
            readData = 0;
            markerData = 0;
        }
    }
}
