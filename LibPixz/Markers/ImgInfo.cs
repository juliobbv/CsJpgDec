using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPixz.Markers;

namespace LibPixz
{
    public class ImgInfo
    {
        public ushort length;
        public byte dataPrecision;
        public ushort height;
        public ushort width;
        public byte numOfComponents;
        public ComponentInfo[] components;

        public HuffmanTable[,] huffmanTables = new HuffmanTable[2, 4];
        public QuantTable[] quantTables = new QuantTable[4];

        public short []deltaDc;
        public short restartMarker;
    }
}
