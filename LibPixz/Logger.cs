using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibPixz
{
    public class Logger
    {
        static StreamWriter writer = new StreamWriter("pixzLog.txt");

        protected internal static void WriteLine()
        {
            writer.WriteLine();
        }

        protected internal static void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        protected internal static void Write(string value)
        {
            writer.Write(value);
        }

        protected internal static void Flush()
        {
            writer.Flush();
        }
    }
}
