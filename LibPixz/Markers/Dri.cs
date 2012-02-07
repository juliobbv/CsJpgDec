using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    class Dri
    {
        public static void Read(BinaryReader reader, ImgInfo imgInfo)
        {
            Logger.WriteLine("---DRI---");
            Logger.WriteLine("Found at: " + reader.BaseStream.Position.ToString("X"));
            Logger.WriteLine();

            int length = reader.ReadBEUInt16();

            imgInfo.restartInterval = reader.ReadBEUInt16();
            Log(reader, imgInfo.restartInterval);
        }

        static void Log(BinaryReader reader, ushort restartInterval)
        {
            Logger.WriteLine("Restart Marker Interval: " + restartInterval);
            Logger.WriteLine();
        }
    }
}
