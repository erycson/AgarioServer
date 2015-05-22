using AgarioServer.Game;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgarioServer.WebSocket
{
    public class Socket
    {
        public event EventHandler<OnRecvEventArgs> OnRecv;
        public event EventHandler<EventArgs> OnDisconnect;

        private IWebSocketConnection _socket;

        public Socket(IWebSocketConnection socket)
        {
            this._socket = socket;
            this._socket.OnClose = () => this.OnDisconnect(null, null);
            this._socket.OnBinary += (buffer) => this.OnRecv(this, new OnRecvEventArgs(buffer));
            this._socket.OnError += (error) =>
            {
                Log.Error(String.Format("Erro from {0}:{1}: {2}", this._socket.ConnectionInfo.ClientIpAddress, this._socket.ConnectionInfo.ClientPort, error.InnerException));
            };
        }

        public async void Send(Byte[] buffer)
        {
            try
            {
                this._socket.Send(buffer);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Error sending data to {0}:{1}: {2}", this._socket.ConnectionInfo.ClientIpAddress, this._socket.ConnectionInfo.ClientPort, ex.InnerException));
            }
        }


        public void Disconnect()
        {
            this._socket.Close();
        }
    }

    public class OnRecvEventArgs : EventArgs
    {
        public Byte[] Buffer { get; private set; }

        public OnRecvEventArgs(Byte[] buffer)
        {
            this.Buffer = buffer;
        }
    }
}
