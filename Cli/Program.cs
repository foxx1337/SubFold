using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SubFold;

namespace Cli
{
    class Program
    {
        private const string PatternNameSrt = @"^(.*)\.srt$";

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Call with a subtitle file as argument.");
                return;
            }

            string fileNameExt = args[0];
            string fileName = NameOnly(fileNameExt);
            
            SrtSubtitle subtitle = new SrtSubtitle(new FileInfo(fileNameExt));
            AssOutput sub3d = new AssOutput(subtitle);
            using (StreamWriter output = new StreamWriter(fileName + ".ass"))
            {
                sub3d.Render(output);
            }
        }

        private static string NameOnly(string fileNameExt)
        {
            Match match = Regex.Match(fileNameExt, PatternNameSrt);
            return match.Groups[1].Value;
        }
    }
}
