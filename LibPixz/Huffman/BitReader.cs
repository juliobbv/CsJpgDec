using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    public class BitReader
    {
        const uint dataSize = sizeof(ushort) * 8;
        const uint readerSize = sizeof(byte) * 8;

        BinaryReader reader;
        uint readData;
        uint availableBits;
        bool dataPad;
        byte restartMarker;

        /// <summary>
        /// How many bits we read at once in the stream
        /// </summary>
        public uint BitStride
        {
            get { return dataSize; }
        }

        public BitReader(BinaryReader reader)
        {
            Flush();
            dataPad = false;

            this.reader = reader;
        }

        public ushort Peek(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            // If we don't have as many bits as needed, read another chunk from stream
            if (length > availableBits)
            {
                byte nextChunk = 0;

                try
                {
                    while (availableBits <= length)
                    {
                        // Restart markers count as a virtual 'stop' when reading from the stream
                        if (restartMarker != 0) break;

                        nextChunk = ReadByteNonStuffed();

                        availableBits += readerSize;
                        readData = (readData << (int)readerSize) | nextChunk;
                    }
                }
                // If already at the end of stream, use only the remaining bits we have read
                // before. Because the number of requested bits is less than the available bits,
                // the result of the clean data has the number of missing bits as zeros appended to
                // the right, as the huffman decoding phase needs a fixed number of bits to work
                catch (Exception)
                {
                    if (dataPad)
                        throw new Exception("Reading two padding chunks, stream may be faulty");

                    dataPad = true;
                }
            }

            // We move data left and right in order to get only the bits we require
            uint cleanData = readData << (int)(dataSize * 2 - availableBits);
            cleanData >>= (int)(dataSize * 2 - length);

            return (ushort)cleanData;
        }

        public ushort Read(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            ushort data = Peek(length);

            availableBits -= length;

            int shift = (int)(dataSize * 2 - availableBits);
            // We move data left and right in order to get only the bits we require
            readData <<= shift;
            readData >>= shift;

            return data;
        }

        public void StopReading()
        {
            // Rewind all (whole) bytes we didn't use
            uint rewind = availableBits / sizeof(byte);

            reader.BaseStream.Seek(-rewind, SeekOrigin.Current);
            Flush();
        }

        public BinaryReader GetBinaryReader()
        {
            return reader;
        }

        public byte ReadByteNonStuffed()
        {
            byte number = reader.ReadByte();

            if (number == 0xff)
            {
                byte markerValue = reader.ReadByte();

                if (markerValue == 0x00)
                {
                    return number;
                }
                else if (markerValue >= (int)Pixz.MarkersId.Rs0 && markerValue <= (int)Pixz.MarkersId.Rs7)
                {
                    restartMarker = markerValue;
                    return 0;
                }
                else
                {
                    return reader.ReadByte();
                }
            }
            else
            {
                return number;
            }
        }

        public bool WasRestartMarkerFound()
        {
            if (restartMarker != 0)
            {
                Flush();
                return true;
            }

            return false;
        }

        public void Flush()
        {
            availableBits = 0;
            readData = 0;
            restartMarker = 0;
        }
    }
}
