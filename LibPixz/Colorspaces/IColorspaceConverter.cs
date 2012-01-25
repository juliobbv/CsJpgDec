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

    public interface IColorspaceConverter
    {
        Color ConvertToRgb(Info info);
        Info ConvertFromRgb(Color rgb);
    }
}