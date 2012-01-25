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
        ushort readData;
        uint remainingBits;

        /// <summary>
        /// How many bits we read at once in the stream
        /// </summary>
        public uint BitStride
        {
            get { return dataSize; }
        }

        public BitReader(BinaryReader reader)
        {
            remainingBits = dataSize;
            this.reader = reader;
            readData = reader.ReadBEUInt16NonStuffed();
        }

        public ushort Read(uint length)
        {
            ushort data = 0;

            if (length > dataSize) throw new Exception("Reading too many bits");

            try
            {
                // If the data is in two integers, then we get the first
                // part of the data, then move it to its correct position
                // and finally combine it with the second part
                if (length > remainingBits)
                {
                    uint splitOffset = length - remainingBits;

                    length -= remainingBits;

                    data = Read(remainingBits);

                    // Data is already clean, so we only put it in the right place
                    data <<= (int)splitOffset;
                }

                // In this part, we attempt to move the data to the left
                // edge, then to the right, in order to clean it from
                // adjacent data
                uint offsetLeft = dataSize - remainingBits;
                uint offsetRight = dataSize - length;

                ushort dataLeft = (ushort)(readData << (int)offsetLeft);
                data |= (ushort)(dataLeft >> (int)offsetRight);

                remainingBits -= length;

                if (remainingBits == 0)
                {
                    remainingBits = dataSize;
                    readData = reader.ReadBEUInt16NonStuffed();
                }

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public BinaryReader GetBinaryReader()
        {
            return reader;
        }
    }
}
