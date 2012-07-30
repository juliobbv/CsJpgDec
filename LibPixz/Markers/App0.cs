using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    class App0 : Marker
    {
        static string name = "APP0";

        public static void Read(BinaryReader reader, ImgInfo imgInfo)
        {
            LogMarker(reader, name);

            ushort length = reader.ReadBEUInt16();
            Logger.WriteLine("Length: " + length.ToString());

            reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
        }
    }
}
