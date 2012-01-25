﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    public struct QuantTable
    {
        public bool valid;
        public byte id;
        public ushort length;
        public byte precision;
        public ushort[] table;
    }

    class Dqt
    {
        public static void Read(BinaryReader reader, ImgInfo imgInfo)
        {
            int markerLength = reader.ReadBEUInt16() - 2;

            while (markerLength > 0)
            {
                int length = ReadTable(reader, imgInfo);
                markerLength -= length;
            }
        }

        public static int ReadTable(BinaryReader reader, ImgInfo imgInfo)
        {
            byte tableInfo = reader.ReadByte();
            byte tableId = (byte)(tableInfo & 0xf); // Low 4 bits of tableInfo

            if (tableId > 3)
                throw new Exception("Invalid ID for quantization table");

            var quantTable = new QuantTable();

            quantTable.id = tableId;
            quantTable.precision = (byte)(tableInfo >> 4); // High 4 bits of tableInfo
            quantTable.valid = true;
            quantTable.table = new ushort[64];

            int sizeOfElement = quantTable.precision == 0 ? 1: 2;

            if (quantTable.precision == 0)
            {
                for (int i = 0; i < 64; i++)
                {
                    quantTable.table[i] = reader.ReadByte();
                }
            }
            else
            {
                for (int i = 0; i < 64; i++)
                {
                    quantTable.table[i] = reader.ReadBEUInt16();
                }
            }

            imgInfo.quantTables[tableId] = quantTable;

            return 1 + 64 * sizeOfElement;
        }
    }
}