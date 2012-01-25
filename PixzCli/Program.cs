using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
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
                DirectoryInfo info = new DirectoryInfo(@"C:\Users\Julio\Dropbox\Pixz\Pics");

                foreach (var file in info.GetFiles())
                {
                }

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