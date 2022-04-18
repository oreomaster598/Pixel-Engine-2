using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE2
{
    public static class Debug
    {
        public static void Log(string text, ConsoleColor color, string sender = "")
        {
            Console.ForegroundColor = color;
            string str = (string.IsNullOrEmpty(sender)) ? text : $"[{sender}] {text}";
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Log(string text, string sender = "")
        {
            string str = (string.IsNullOrEmpty(sender)) ? text : $"[{sender}] {text}";
            Console.WriteLine(str);
        }
        public static void LogError(string text, string sender = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string str = (string.IsNullOrEmpty(sender)) ? text : $"[{sender}] {text}";
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void LogWarning(string text, string sender = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string str = (string.IsNullOrEmpty(sender)) ? text : $"[{sender}] {text}";
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
