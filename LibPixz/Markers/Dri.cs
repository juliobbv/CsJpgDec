using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    class Dri
    {
        public const int RestartMarkerPeriod = 8;

        public static void Read(BinaryReader reader, ImgInfo imgInfo)
        {
            Logger.WriteLine("---DRI---");
            Logger.WriteLine("Found at: " + reader.BaseStream.Position.ToString("X"));
            Logger.WriteLine();

            int length = reader.ReadBEUInt16();

            ushort restartInterval = reader.ReadBEUInt16();

            if (restartInterval == 0)
                throw new Exception("Invalid restart interval (0)");

            imgInfo.restartInterval = restartInterval;
            imgInfo.hasRestartMarkers = true;

            Log(reader, imgInfo.restartInterval);
        }

        static void Log(BinaryReader reader, ushort restartInterval)
        {
            Logger.WriteLine("Restart Marker Interval: " + restartInterval);
            Logger.WriteLine();
        }
    }
}
