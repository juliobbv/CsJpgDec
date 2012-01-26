using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    /// <summary>
    /// Extensions for BinaryReader that enable reading big endian numbers,
    /// </summary>
    public static class BinaryReaderEx
    {
        public static ushort ReadBEUInt16(this BinaryReader reader)
        {
            byte upperByte = reader.ReadByte();
            byte lowerByte = reader.ReadByte();

            return (ushort)(upperByte << 8 | lowerByte);
        }
    }
}
