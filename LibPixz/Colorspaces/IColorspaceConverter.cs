using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LibPixz.Colorspaces
{
    public struct Info
    {
        public float a;
        public float b;
        public float c;
    }

    public struct Color2
    {
        public byte a;
        public byte r;
        public byte g;
        public byte b;
    }

    public interface IColorspaceConverter
    {
        Color2 ConvertToRgb(Info info);
        Info ConvertFromRgb(Color2 rgb);
    }
}