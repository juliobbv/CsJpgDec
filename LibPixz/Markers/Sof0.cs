using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    public struct ComponentInfo
    {
        public byte samplingFactorX;
        public byte samplingFactorY;
        public byte quantTableId;
        public byte dcHuffmanTable;
        public byte acHuffmanTable;
    }

    class Sof0
    {
        public static void Read(BinaryReader reader, ImgInfo imgInfo)
        {
            imgInfo.length = reader.ReadBEUInt16();
            imgInfo.dataPrecision = reader.ReadByte();
            imgInfo.height = reader.ReadBEUInt16();
            imgInfo.width = reader.ReadBEUInt16();
            imgInfo.numOfComponents = reader.ReadByte();

            if (imgInfo.length < 8)
                throw new Exception("Invalid length of Sof0");

            if (imgInfo.height == 0 || imgInfo.width == 0)
                throw new Exception("Invalid image size");

            if (imgInfo.dataPrecision != 8)
                throw new Exception("Unsupported data precision");

            if (imgInfo.numOfComponents != 1 && imgInfo.numOfComponents != 3)
                throw new Exception("Invalid number of components");

            imgInfo.components = new ComponentInfo[imgInfo.numOfComponents];

            for (int i = 0; i < imgInfo.numOfComponents; i++)
            {
                byte id = (byte)(reader.ReadByte() - 1);

                if (id > 2)
                    throw new Exception("Invalid component type");

                byte samplingFactor = reader.ReadByte();

                imgInfo.components[id].samplingFactorX = (byte)(samplingFactor & 0x0f);
                imgInfo.components[id].samplingFactorY = (byte)(samplingFactor >> 4);

                imgInfo.components[id].quantTableId = reader.ReadByte();
            }
        }
    }
}
