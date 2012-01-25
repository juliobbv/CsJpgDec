using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    /// <summary>
    /// Extensions for BinaryReader that enable reading big endian numbers,
    /// or numbers without markers
    /// </summary>
    public static class BinaryReaderEx
    {
        public static ushort ReadBEUInt16(this BinaryReader reader)
        {
            byte upperByte = reader.ReadByte();
            byte lowerByte = reader.ReadByte();

            return (ushort)(upperByte << 8 | lowerByte);
        }

        public static uint ReadBEUInt32NonStuffed(this BinaryReader reader)
        {
            uint output = 0;

            for (int i = 0; i < 4; i++)
            {
                byte theByte = reader.ReadByte();

                if (theByte == 0xff) reader.ReadByte();

                output = (output << 8) | theByte;
            }

            return output;
        }

        public static ushort ReadBEUInt16NonStuffed(this BinaryReader reader)
        {
            byte upperByte = reader.ReadByte();

            if (upperByte == 0xff) reader.ReadByte();

            byte lowerByte = reader.ReadByte();

            if (lowerByte == 0xff) reader.ReadByte();

            return (ushort)(upperByte << 8 | lowerByte);
        }

        public static byte ReadByteNonStuffed(this BinaryReader reader)
        {
            byte number = reader.ReadByte();

            if (number == 0xff) reader.ReadByte();

            return number;
        }
    }
}
