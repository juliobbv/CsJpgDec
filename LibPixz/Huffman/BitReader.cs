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
        BinaryReader reader;
        uint readData;
        uint availableBits;

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
            this.reader = reader;
        }

        public ushort Peek(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            // If we don't have as many bits as needed, read another chunk from stream
            if (length > availableBits)
            {
                availableBits += dataSize;

                ushort nextChunk = 0;

                try
                {
                    nextChunk = reader.ReadBEUInt16NonStuffed();
                }
                // If already at the end of stream, next chunk will be all zeros,
                // so we can decode the last blocks of the image
                catch (Exception) { }

                readData = (readData << 16) | nextChunk;
            }

            // We move data left and right in order to get only the bits we require
            uint cleanData = readData << (int)(dataSize * 2 - availableBits);
            cleanData >>= (int)(dataSize * 2 - length);

            return (ushort)cleanData;
        }

        public ushort Read(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            ushort data = Peek(length);

            availableBits -= length;

            int shift = (int)(dataSize * 2 - availableBits);
            // We move data left and right in order to get only the bits we require
            readData <<= shift;
            readData >>= shift;

            return data;
        }

        public BinaryReader GetBinaryReader()
        {
            return reader;
        }
    }
}
