using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    class Default : Marker
    {
        public static void Read(BinaryReader reader, ImgInfo imgInfo, Pixz.MarkersId markerId)
        {
            Logger.Write("Unknown marker (" + markerId.ToString("X") + ")");

            if (!imgInfo.startOfImageFound)
            {
                Logger.Write(" found outside of image");
            }

            Logger.WriteLine(" at: " + (reader.BaseStream.Position - 2).ToString("X"));

            // Check if marker is not followed by a length argument
            if (markerId >= Pixz.MarkersId.Rs0 && markerId <= Pixz.MarkersId.Rs7)
                return;
            if (markerId == Pixz.MarkersId.LiteralFF)
                return;

            if (!imgInfo.startOfImageFound) return;

            ushort length = reader.ReadBEUInt16();
            Logger.WriteLine("Length: " + length.ToString());

            reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
        }
    }
}
