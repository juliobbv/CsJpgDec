﻿using System;
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
            Logger.WriteLine(" at: " + reader.BaseStream.Position.ToString("X"));

            // Check if marker is not followed by a length
            if (markerId >= Pixz.MarkersId.Rs0 && markerId <= Pixz.MarkersId.Rs7)
                return;
            if (markerId == Pixz.MarkersId.Literal255)
                return;

            ushort length = reader.ReadBEUInt16();
            Logger.WriteLine("Length: " + length.ToString());

            reader.BaseStream.Seek(length - 2, SeekOrigin.Current);
        }
    }
}
