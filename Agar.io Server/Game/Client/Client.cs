using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Windows;
using System.Threading;
using System.Collections.Generic;

using AgarioServer.WebSocket;
using AgarioServer.Game.Packet;
using Fleck;

namespace AgarioServer.Game.Clients
{
    public class Client
    {
        protected static KnownColor[] Colors = (KnownColor[])Enum.GetValues(typeof(KnownColor));
        private static UInt32 _Id = 1;

        // Propriedades do objeto
        public UInt32 Id;
        public System.Windows.Point Position;
        public Single Size { get; private set; }
        public Color Color;

        private UInt32 _score;
        public UInt32 Score
        {
            set
            {
                this._score = value;
                this.Size = (Single) Math.Sqrt(value * 100);
            }
            get { return this._score; }
        }

        // Propriedades da classe
        public ClientType Type { get; private set; }

        protected Socket _socket;

        public Realm Realm;

        public Client(ClientType type)
        {
            this.Id = Client._Id++;
            this.Type = type;
        }

        public Client(UInt32 id, ClientType type)
        {
            this.Id = id;
            this.Type = type;
        }

        public async void Send(Byte[] data)
        {
            if (this._socket != null)
                this._socket.Send(data);
        }

        public Boolean InsideClient(System.Windows.Point position)
        {
            Vector distance = this.Position - position;
            return distance.Length < this.Size * 0.9;
        }
    }

    public enum ClientType
    {
        Cell, 
        Food, 
        Virus
    }
}
