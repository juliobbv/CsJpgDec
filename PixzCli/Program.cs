using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using LibPixz;

namespace PixzCli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("test"))
            {
                Console.WriteLine("Testing 123");
                Test();
            }
        }

        static bool Test()
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(@"C:\Users\Julio\Dropbox\CsJpgDec\Pics");
                Stopwatch watch = new Stopwatch();

                watch.Start();
                foreach (var file in info.GetFiles())
                {
                    Console.WriteLine(file.FullName);
                    Pixz.Decode(file.FullName);
                }
                watch.Stop();

                Console.WriteLine("Test took: " + watch.ElapsedMilliseconds / 1000.0 + " s");
                Console.ReadKey();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: :(" + ex.Message);
                Console.ReadKey();
                return false;
            }
        }
    }
}