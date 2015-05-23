using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.IO;

using AgarioServer.Game;

namespace AgarioServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://paulbatum.github.io/WebSocket-Samples/HttpListenerWebSocketEcho/
            new Program();
        }

        private Program()
        {
            Log.Info("Loading settings");
            Config.Load();

            AppDomain.CurrentDomain.UnhandledException += (s, e) => {
                Log.Fatal(e.ExceptionObject.ToString());
            };

            Log.Info("Connecting to database");
            Database.Instance.Connect();

            String region = Database.Instance.GetServerRegion(Config.ServerId);
            if (region == String.Empty)
                Log.Fatal("Server region not found");

            Database.Instance.CleanServerStatus(Config.ServerId);

            String listen = String.Format("ws://{0}:{1}/", Config.Host, Config.Port);
            Server server = new Server(region);
            server.Start(listen);

            Database.Instance.ServerStatus(Config.ServerId, Config.Host + ":" + Config.Port, true);

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Database.Instance.ServerStatus(Config.ServerId, null, false);
            };

            while(true)
                Console.ReadKey();
        }
    }
}
