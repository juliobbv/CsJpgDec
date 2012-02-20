using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    internal class Logger
    {
        static StreamWriter writer = new StreamWriter("pixzLog.txt");

        internal static void WriteLine()
        {
            writer.WriteLine();
        }

        internal static void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        internal static void Write(string value)
        {
            writer.Write(value);
        }

        internal static void Flush()
        {
            writer.Flush();
        }
    }
}
