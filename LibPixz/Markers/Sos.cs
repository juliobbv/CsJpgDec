using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace LibPixz.Markers
{
    class Sos : Marker
    {
        static string name = "SOS";

        public static Bitmap Read(BinaryReader reader, ImgInfo imgInfo)
        {
            LogMarker(reader, name);

            if (imgInfo.numOfComponents != 1 && imgInfo.numOfComponents != 3)
                throw new Exception("Unsupported format");

            ushort length = reader.ReadBEUInt16();
            byte componentsInScan = reader.ReadByte();

            for (int i = 0; i < componentsInScan; i++)
            {
                byte componentId = (byte)(reader.ReadByte() - 1);
                byte huffmanTables = reader.ReadByte();

                byte acTable = (byte)(huffmanTables & 0xf);
                byte dcTable = (byte)(huffmanTables >> 4);

                imgInfo.components[componentId].dcHuffmanTable = dcTable;
                imgInfo.components[componentId].acHuffmanTable = acTable;
            }

            reader.ReadBytes(3); // "Unused" bytes

            return ImageDecoder.DecodeImage(reader, imgInfo);
        }
    }
}
