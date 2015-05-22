using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AgarioServer.Game.Clients;

namespace AgarioServer.Game.Packet
{
    /// <summary>
    ///     Envia os dados dos jogadores
    /// </summary>
    class Packet16 : Packet
    {
        private List<Client> _clients;

        public Packet16() : base(16)
        {
            this._clients = new List<Client>();
            this._clients.Capacity = 3000;
        }

        public void AddClient(Client client)
        {
            this._clients.Add(client);
        }

        public void AddClients(Client[] clients)
        {
            this._clients.AddRange(clients);
        }

        public Byte[] ToByteArray()
        {
            // Destruir
            this.Writer.Write((UInt16)0); // Quantidade
            // 1 - Cliente
            //this.Writer.Write((UInt32)1); // ID
            //this.Writer.Write((UInt32)1); // Destruir?

            foreach (Client client in this._clients)
            {
                this.Writer.Write(client.Id);
                this.Writer.Write((Single)client.Position.X);
                this.Writer.Write((Single)client.Position.Y);
                this.Writer.Write(client.Size);
                this.Writer.Write(client.Color.R);
                this.Writer.Write(client.Color.G);
                this.Writer.Write(client.Color.B);

                if (client.Type == ClientType.Cell)
                {
                    Cell c = (Cell)client;
                    this.Writer.Write((Byte)0);

                    // Verifica se o nome está vazio
                    if (c.Name != null)
                        this.Writer.Write(c.Name);
                }
                else if (client.Type == ClientType.Food)
                {
                    this.Writer.Write((Byte)0);
                    // Comida
                }
                else if (client.Type == ClientType.Virus)
                {
                    this.Writer.Write((Byte)1);
                    // Virus
                }

                // Avisa que não tem nome ou que acabou o nome
                this.Writer.Write((UInt16)0);
            }

            // Para o for de clientes
            this.Writer.Write((UInt32)0);

            // Nada 
            this.Writer.Write((UInt16)0);

            // Quantidade
            this.Writer.Write((UInt32)0); 

            return base.ToByteArray();
        }
    }
}
