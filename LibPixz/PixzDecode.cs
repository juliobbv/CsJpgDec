using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using LibPixz.Colorspaces;
using LibPixz.Markers;

namespace LibPixz
{
    public partial class Pixz
    {
        public enum Markers
        {
            Soi = 0xd8,
            App0 = 0xe0,
            Dqt = 0xdb,
            Sof0 = 0xc0,
            Dht = 0xc4,
            Sos = 0xda,
            Eoi = 0xd9
        }

        public static List<Bitmap> Decode(string path)
        {
            MemoryStream stream = ReadFileToMemory(path);

            return Pixz.Decode(stream);
        }

        public static List<Bitmap> Decode(MemoryStream stream)
        {
            var reader = new BinaryReader(stream);
            var images = new List<Bitmap>();

            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                bool eof = false;

                for (int image = 1; ; image++)
                {
                    ImgInfo imgInfo = new ImgInfo();

                    while (true)
                    {
                        while (reader.ReadByte() != 0xff) ;
                        int markerId = reader.ReadByte();

                        switch ((Markers)markerId)
                        {
                            case Markers.App0:
                                break;
                            case Markers.Dqt:
                                Dqt.Read(reader, imgInfo);
                                break;
                            case Markers.Sof0:
                                Sof0.Read(reader, imgInfo);
                                break;
                            case Markers.Dht:
                                Dht.Read(reader, imgInfo);
                                break;
                            case Markers.Sos:
                                images.Add(Sos.Read(reader, imgInfo));
                                break;
                            case Markers.Soi:
                                Logger.Write("Start of Image " + image);
                                Logger.WriteLine(" at: " + reader.BaseStream.Position.ToString("X"));
                                break;
                            case Markers.Eoi:
                                eof = true;
                                break;
                            default:
                                Logger.Write("Unknown marker (" + markerId.ToString("X") + ")");
                                Logger.WriteLine(" at: " + reader.BaseStream.Position.ToString("X"));
                                break;
                        }

                        if (eof)
                        {
                            eof = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

            reader.Close();
            Logger.Flush();

            //bmp.Save("C:\\progress1.png");
            return images;
        }

        protected static MemoryStream ReadFileToMemory(string path)
        {
            MemoryStream stream = new MemoryStream();
            FileStream archivo = File.OpenRead(path);

            stream.SetLength(archivo.Length);
            archivo.Read(stream.GetBuffer(), 0, (int)archivo.Length);

            archivo.Close();

            return stream;
        }
    }
}
