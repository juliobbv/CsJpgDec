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

        public static Bitmap Decode(string path, EncodeType tipo)
        {
            MemoryStream stream = LeerArchivo(path);

            return Pixz.Decode(stream, tipo);
        }

        public static Bitmap Decode(MemoryStream stream, EncodeType tipo)
        {
            var reader = new BinaryReader(stream);
            ImgInfo imgInfo = new ImgInfo();
            Bitmap bmp = null;

            stream.Seek(0, SeekOrigin.Begin);

            while (reader.ReadByte() != 0xff) ;

            if ((Markers)reader.ReadByte() != Markers.Soi)
                throw new Exception("Invalid JPEG file");

            try
            {
                bool eof = false;

                while (true)
                {
                    while (reader.ReadByte() != 0xff) ;

                    switch ((Markers)reader.ReadByte())
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
                            bmp = Sos.Read(reader, imgInfo);
                            break;
                        case Markers.Eoi:
                            eof = true;
                            break;
                        default:
                            break;
                    }

                    if (eof) break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

            reader.Close();
            Logger.Flush();

            //bmp.Save("C:\\progress1.png");
            return bmp;
        }

        protected static MemoryStream LeerArchivo(string path)
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
