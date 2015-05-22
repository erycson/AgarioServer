using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AgarioServer.Game.Clients;
using AgarioServer.Game.Exceptions;
using AgarioServer.Game.Packet;
using AgarioServer.WebSocket;

namespace AgarioServer.Game
{
    public class Server
    {
        public List<Realm> _realms { get; private set; }
        private String _name;
        private Byte _realmCount;
        private WebSocket.Server _server;
        private Random _random = new Random();

        public Server(String name)
        {
            this._realmCount = 1;
            this._name = name;
            this._realms = new List<Realm>();
        }

        public Realm GetRealm()
        {
            Realm realm = null;

            // Verifica se existe algum reino disponivel
            foreach (Realm r in this._realms)
            {
                // Verifica se o server está cheio
                if (r.Clients.Count == 150)
                    continue;

                realm = r;
                break;
            }

            // Se não existir reino disponivel, cria um e adiciona o cliente
            if (realm == null)
            {
                realm = new Realm(this._realmCount++);
                this._realms.Add(realm);
            }

            return realm;
        }

        public void Start(String prefix)
        {
            Log.Info("Starting Server " + prefix);
            this._server = new WebSocket.Server(prefix);
            this._server.OnConnect += OnConnect;
            this._server.Start();
            Log.Info("Server Started");

        }

        private void OnConnect(object sender, OnConnectEventArgs e)
        {
            Realm realm = this.GetRealm();
            Cell cell = new Cell(e.Socket);
            realm.AddCell(cell);
        }
    }
}
