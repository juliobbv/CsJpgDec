using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz.Markers
{
    public abstract class Marker
    {
        protected static void LogMarker(BinaryReader reader, string name)
        {
            Logger.WriteLine("---" + name + "---");
            Logger.WriteLine("Found at: " + (reader.BaseStream.Position - 2).ToString("X"));
            Logger.WriteLine();
        }
    }
}
