using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LutherBackup
{
    public static class ConsoleHelpers
    {
        public static void WriteSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteMessage(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        public static void WriteError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();        
        }

        public static void WriteWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }


        public static void WriteEmptyLine()
        {
            Console.WriteLine();
        }


    }
}
