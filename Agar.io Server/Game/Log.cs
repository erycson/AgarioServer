using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game
{
    public static class Log
    {
        private static String INFO = "INFO";
        private static String WARNING = "WARNING";
        private static String ERROR = "ERROR";
        private static String FATAL = "FATAL";
        private static String DEBUG = "DEBUG";
        private static StreamWriter _sw = File.AppendText("log.txt");

        public static void Fatal(String message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log.WriteEntry(Log.FATAL, message);
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void Error(String message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log.WriteEntry(Log.ERROR, message);
        }

        public static void Debug(String message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log.WriteEntry(Log.DEBUG, message);
        }

        public static void Info(String message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log.WriteEntry(Log.INFO, message);
        }

        public static void Warning(String message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.WriteEntry(Log.WARNING, message);
        }

        private static void WriteEntry(String level, String message)
        {
            String log = String.Format("[{0}] {1} - {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                level,
                message
            );
            Console.WriteLine(log);
            Log._sw.WriteLine(log);
        }

        internal static void Info()
        {
            throw new NotImplementedException();
        }
    }
}
