using ParserLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerDatFixer_Framework
{
    class Program
    {
        static readonly Version Version = new Version(0, 3, 0);
        static void Close()
        {
            HorizontalLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
        static void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
        }
        static void Info(string message)
        {
            Console.WriteLine("[INFO] " + message);
        }
        static void HorizontalLine()
        {
            Console.WriteLine("==============================================================================");
        }

        static void Main(string[] args)
        {
            HorizontalLine();
            Info($"VERSION: {Version}");
            Info("Made by Sc2ad");
            Info("Github repo: https://github.com/sc2ad/PlayerDataFixer");
            HorizontalLine();
            if (args.Length == 0)
            {
                Error("Please drag a file or folder onto this application!");
                Close();
                return;
            }
            Parser.PerformCopy(args[0], Info, Error);
            Close();
        }
    }
}
