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
        private const String INFO = "INFO";
        private const String WARNING = "WARNING";
        private const String ERROR = "ERROR";
        private const String FATAL = "FATAL";
        private const String DEBUG = "DEBUG";

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
            String log = String.Format("[{0}] {1} - {2}" + Environment.NewLine,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                level,
                message
            );
            Console.Write(log);

            if (!File.Exists("log.txt"))
                File.WriteAllText("log.txt", log);
            else
                File.AppendAllText("log.txt", log);
        }
    }
}
