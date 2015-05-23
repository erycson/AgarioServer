using AgarioServer.Game.Packet;
using AgarioServer.WebSocket;
using Fleck;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace AgarioServer.Game.Clients
{
    public class Cell : Client
    {
        public UInt32 OwnerID;
        public Byte[] Name;
        public Vector Direction;
        public Boolean Connected;
        private Timer _timer;
        private Timer _timerSalt;
        private Int32 _salts = 0;

        public Cell(Socket socket)
            : base(ClientType.Cell)
        {
            this.OwnerID = this.Id;
            this.Score = 10;
            this.Connected = false;

            this._socket = socket;
            this._socket.OnRecv += OnRecv;
            this._socket.OnDisconnect += OnDisconnect;

            this._timer = new Timer(1000/Config.IntervalPosition);
            this._timer.Elapsed += CalculateMovement;

            Random rdm = new Random();
            this.Color = Color.FromKnownColor(Client.Colors[rdm.Next(Client.Colors.Length)]);
        }

        public Cell(UInt32 ownerId, Socket socket, Byte[] name, Color color, UInt32 points, Vector direction, System.Windows.Point position)
            : base(ClientType.Cell)
        {
            this.OwnerID = ownerId;
            this.Score = points;
            this.Color = color;
            this.Direction = direction;
            this.Name = name;
            this.Position = position;

            this._socket = socket;
            this._socket.OnRecv += OnRecv;
            this._socket.OnDisconnect += OnDisconnect;

            this._timerSalt = new Timer(1000 / Config.IntervalPosition);
            this._timerSalt.Elapsed += MoveCell;
            this._timerSalt.Start();

            this._timer = new Timer(1000 / Config.IntervalPosition);
            this._timer.Elapsed += CalculateMovement;

            this.Connected = true;

            Packet32 p32 = new Packet32();
            p32.SetId(this.Id);
            this.Send(p32.ToByteArray());
        }

        private void MoveCell(object sender, ElapsedEventArgs e)
        {
            this.Position = this.Position + this.Direction * (40 - this._salts);

            if (this._salts++ > 10)
                this._timerSalt.Stop();
        }

        public void SetRealm(Realm realm)
        {
            this.Realm = realm;
            this._timer.Start();
        }

        private void OnConnect(BinaryReader packet)
        {
            Console.WriteLine("Client {0} connected", this.Id);

            Packet64 p64 = new Packet64();
            p64.SetPosition(this.Realm.X, this.Realm.Y);
            p64.SetSize(this.Realm.Width, this.Realm.Height);
            this.Send(p64.ToByteArray());
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            Log.Info(String.Format("Player {0} has disconnected", this.Id));
            this.Realm.RemoveCell(this);
            if (this.Connected)
                Database.Instance.RemovePlayer(Config.ServerId);
        }

        private void Destroy()
        {
            List<Client> clients = new List<Client>(this.Realm.Clients);

            // Verifica quantas celulas pertence a esse jogador
            Int32 count = clients.OfType<Cell>()
                .Where(x => x.Id != this.Id && x.OwnerID == this.OwnerID)
                .Count();

            this.Realm.RemoveCell(this);

            if (count == 0)
            {
                //this._socket.Disconnect();
                this.Send(new Packet20().ToByteArray());
                this.Send(new Packet32().SetId(0).ToByteArray());

                if (this.Connected)
                {
                    this.Connected = false;
                    Database.Instance.RemovePlayer(Config.ServerId);
                }
            }
        }

        private void OnRecv(object sender, OnRecvEventArgs e)
        {
            BinaryWriter bufWriter = new BinaryWriter(new MemoryStream());
            bufWriter.Write(e.Buffer);

            BinaryReader bufReader = new BinaryReader(bufWriter.BaseStream);
            bufReader.BaseStream.Seek(0, SeekOrigin.Begin);

            Byte type = bufReader.ReadByte();
            switch (type)
            {
                case 0:
                    this.OnName(bufReader);
                    break;
                case 16:
                    this.OnMouseMove(bufReader);
                    break;
                case 17:
                    this.OnSplitCell(bufReader);
                    break;
                case 21:
                    this.OnEjectMass(bufReader);
                    break;
                case 255:
                    this.OnConnect(bufReader);
                    break;
                default:
                    Int32 length = (Int32)bufReader.BaseStream.Length;
                    Byte[] data = bufReader.ReadBytes(length);
                    Log.Debug(String.Format("Unkown packet {0} from player {1}: {2}", type, this.Id, BitConverter.ToString(data)));
                    break;
            }
        }

        private void OnEjectMass(BinaryReader packet)
        {
            if (this.Score < 32)
                return;

            this.Score -= 16;

            Food food = new Food(this.Direction);
            food.Score = 16;
            food.Position = this.Position + this.Direction * this.Size * 2;
            food.Color = this.Color;

            this.Realm.AddFood(food);
        }

        private void OnSplitCell(BinaryReader packet)
        {
            if (this.Score < 36)
                return;

            Cell cell = new Cell(this.OwnerID, this._socket, this.Name, this.Color, (UInt32)Math.Floor(this.Score / 2.0), this.Direction, this.Position + this.Direction * this.Size * 2);
            this.Realm.AddCell(cell, false);

            this.Score -= (UInt32)Math.Ceiling(this.Score / 2.0);
        }
        
        private void OnMouseMove(BinaryReader packet)
        {
            System.Windows.Point mouse = new System.Windows.Point(packet.ReadDouble(), packet.ReadDouble());
            this.Direction = mouse - this.Position;
            this.Direction.Normalize();
        }

        private void OnName(BinaryReader packet)
        {
            Byte[] data;
            List<Byte> name = new List<Byte>();
            do
            {
                data = packet.ReadBytes(2);
                if (data.Length != 2)
                    break;
                name.AddRange(data);
            } while (true);

            if (name.Count > 0)
                this.Name = name.ToArray();

            this.Connected = true;
            Database.Instance.AddPlayer(Config.ServerId);

            Packet32 p32 = new Packet32();
            p32.SetId(this.Id);
            this.Send(p32.ToByteArray());

            Log.Info(String.Format("Player {0} entered the realm {1}", this.Id, this.Realm.Id));
        }

        /// <summary>
        ///     Mais informações sobre o calculo: http://stackoverflow.com/questions/6203262/c-sharp-xna-calculating-next-point-in-a-vector-direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateMovement(object sender, ElapsedEventArgs e)
        {
            Double speed = 15 * Math.Pow(1 / 1.001, this.Size);
            this.Position = this.Position + this.Direction * speed;
            Single radius = this.Size * 0.6f;

            // impede de sair da tela
            if (this.Position.X < this.Realm.X + radius)
                this.Position.X = this.Realm.X + radius;
            else if (this.Position.X > this.Realm.Width - radius)
                this.Position.X = this.Realm.Width - radius;

            if (this.Position.Y < this.Realm.Y + radius)
                this.Position.Y = this.Realm.Y + radius;
            else if (this.Position.Y > this.Realm.Height - radius)
                this.Position.Y = this.Realm.Height - radius;
            
            if (this.Realm != null)
            {
                Food f;
                Cell c;

                List<Client> clients = new List<Client>(this.Realm.Clients);
                clients = clients.Where(x => x.Id != this.Id).ToList();

                foreach(Client client in clients)
                {
                     if (client.Type == ClientType.Food && this.InsideClient(client.Position))
                     {
                         f = (Food)client;
                         f.StopMove();
                         this.Score += f.Score;
                         this.Realm.RemoveFood(f);
                     }
                     else if (client.Type == ClientType.Cell)
                     {
                         c = (Cell)client;

                         // Verifica se não é uma celula dele mesmo
                         if (c.OwnerID != this.OwnerID)
                         {
                             // Se esse cliente for maior
                             if (this.InsideClient(c.Position) && this.Score * 1.25 > c.Score && c.Connected)
                             {
                                 this.Score += c.Score;
                                 c.Connected = false;
                                 c.Destroy();
                             }
                             // Se esse cliente for menor
                             else if (c.InsideClient(this.Position) && c.Score * 1.25 > this.Score && this.Connected)
                             {
                                 c.Score += this.Score;
                                 this.Connected = false;
                                 this.Destroy();
                             }
                         }
                         else
                         {
                             if (this.InsideClient(c.Position))
                             {
                                 Vector direction = this.Position - c.Position;
                                 direction.Negate();
                                 direction.Normalize();
                                 this.Position = this.Position - direction * (this.Size + c.Size - direction.Length);
                             }
                         }
                     }
                 }
            }
        }
    }
}
