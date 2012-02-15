using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    /// <summary>
    /// Restart marker-aware bit reader for JPEG decoding
    /// </summary>
    public class BitReader
    {
        const int dataSize = sizeof(ushort) * 8;
        const int readerSize = sizeof(byte) * 8;

        BinaryReader reader;
        uint readData;
        int availableBits;
        bool dataPad;
        byte restartMarker;

        /// <summary>
        /// Returns how many bits we read at once in the stream
        /// </summary>
        public uint BitStride
        {
            get { return dataSize; }
        }

        /// <summary>
        /// Returns true if end of file has been reached and reader is padding with zeros
        /// </summary>
        public bool PastEndOfFile
        {
            get { return dataPad && availableBits <= 0; }
        }

        /// <summary>
        /// Returns the BinaryReader the BitReader is using
        /// </summary>
        public BinaryReader BaseBinaryReader
        {
            get { return reader; }
        }

        public BitReader(BinaryReader reader)
        {
            Flush();
            dataPad = false;

            this.reader = reader;
        }

        /// <summary>
        /// Peeks a certain number of bits from a certain stream
        /// </summary>
        /// <param name="length">The number of bits we want to read from the stream</param>
        /// <returns>An unsigned 2 byte integer containing the requested bits, with enough leading
        /// zeros to pad the result, and trailing zeros to fill the requested length if past the
        /// end of stream
        /// </returns>
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
                        // Restart markers block reads from the stream until we call flush
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
                catch (EndOfStreamException)
                {
                    dataPad = true;
                }
            }

            // We move data left and right in order to get only the bits we require
            uint cleanData = readData << (int)(dataSize * 2 - availableBits);
            cleanData >>= (int)(dataSize * 2 - length);

            return (ushort)cleanData;
        }

        /// <summary>
        /// Reads a certain number of bits from a certain stream
        /// </summary>
        /// <param name="length">The number of bits we want to read from the stream</param>
        /// <returns>An unsigned 2 byte integer containing the requested bits, with enough leading
        /// zeros to pad the result, and trailing zeros to fill the requested length if past the
        /// end of stream
        /// </returns>
        public ushort Read(uint length)
        {
            if (length > dataSize) throw new Exception("Reading too many bits");

            ushort data = Peek(length);

            availableBits -= (int)length;

            int shift = (int)(dataSize * 2 - availableBits);
            // We move data left and right in order to get only the bits we require
            readData <<= shift;
            readData >>= shift;

            return data;
        }

        /// <summary>
        /// Flush all data in the buffer and rewinds all readahead bytes
        /// </summary>
        public void StopReading()
        {
            if (!dataPad)
            {
                // Rewind all (whole) bytes we didn't use
                int rewind = availableBits / sizeof(byte);

                reader.BaseStream.Seek(-rewind, SeekOrigin.Current);
            }

            Flush();
        }

        /// <summary>
        /// Deletes all data in the buffer, without rewinding all readahead bytes
        /// in stream
        /// </summary>
        public void Flush()
        {
            availableBits = 0;
            readData = 0;
            restartMarker = 0;
        }

        private byte ReadByteNonStuffed()
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

        /// <summary>
        /// Finds the next restart marker
        /// </summary>
        /// <returns>The next restart marker</returns>
        public Pixz.MarkersId SyncStreamToNextRestartMarker()
        {
            while (restartMarker == 0)
            {
                ReadByteNonStuffed();
            }

            byte restartM = restartMarker;
            Flush();

            return (Pixz.MarkersId)restartM;
        }
    }
}
