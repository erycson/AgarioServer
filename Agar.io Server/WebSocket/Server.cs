using AgarioServer.Game;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.WebSocket
{
    public class Server
    {
        public event EventHandler<OnConnectEventArgs> OnConnect;

        private WebSocketServer _listener;
        private delegate void OnRequestEventHandler();

        public Server(String prefix)
        {
            this._listener = new WebSocketServer(prefix);
        }

        public void Start()
        {
            this._listener.Start(socket =>
            {
                Log.Info(String.Format("New connection from {0}:{1}", socket.ConnectionInfo.ClientIpAddress, socket.ConnectionInfo.ClientPort));
                this.OnConnect(this, new OnConnectEventArgs(new Socket(socket)));
            });
        }
    }

    public class OnConnectEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }

        public OnConnectEventArgs(Socket socket)
        {
            this.Socket = socket;
        }
    }
}
