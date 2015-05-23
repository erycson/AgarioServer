using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using AgarioServer.Game.Packet;
using AgarioServer.Game.Clients;
using AgarioServer.Game.Exceptions;

namespace AgarioServer.Game
{
    public class Realm
    {
        public Byte Id;

        private Timer _timerPosition;
        private Timer _timerFood;
        private Timer _timerVirus;
        private Timer _timerRanking;
        public List<Client> Clients;

        public Double X = 0;
        public Double Y = 0;
        public Double Width = Config.RealmWidth;
        public Double Height = Config.RealmHeight;
        

        public static Byte MaxClients = 150;
        private Int32 FoodCount = 0;
        private Int32 VirusCount = 0;
        private Random _rdm = new Random();

        public Realm(Byte id)
        {
            this.Id = id;
            Log.Info("Realm " + this.Id + " created");

            this.Clients = new List<Client>();
            this.Clients.Capacity = 5000;

            this.CreateFood(null, null);
            this.CreateVirus(null, null);

            // Envia a posição numa taxa de 30 frames por segundo
            this._timerPosition = new Timer(Config.IntervalPosition);
            this._timerPosition.Elapsed += SendClientPositions;
            this._timerPosition.Start();

            // Recria os virus a cada 1 min
            this._timerFood = new Timer(Config.IntervalFood);
            this._timerFood.Elapsed += CreateFood;
            this._timerFood.Start();

            // Recria os virus a cada 5 min
            this._timerVirus = new Timer(Config.IntervalVirus);
            this._timerVirus.Elapsed += CreateVirus;
            this._timerVirus.Start();

            // Envia o ranking a cada 0,5 segundo
            this._timerRanking = new Timer(Config.IntervalRanking);
            this._timerRanking.Elapsed += SendCellRanking;
            this._timerRanking.Start();

            Database.Instance.AddRealm(Config.ServerId);
        }

        public void AddFood(Food food)
        {
            food.Realm = this;
            this.Clients.Add(food);
            this.FoodCount++;
        }

        public void AddVirus(Virus virus, Boolean fromSlipt = false)
        {
            virus.Realm = this;
            this.Clients.Add(virus);
            if (!fromSlipt)
                this.VirusCount++;
        }

        public void AddCell(Cell cell, Boolean calcPosition = true)
        {
            cell.SetRealm(this);
            if (calcPosition)
                cell.Position = this.GetRandomPoint();

            this.Clients.Add(cell);
        }

        public Int32 GetClientCount()
        {
            return this.Clients.Count;
        }

        public void RemoveCell(Cell cell)
        {
            this.Clients.Remove(cell);
        }

        /// <summary>
        ///     Remove uma comida de dentro do reino
        /// </summary>
        /// <param name="id">ID da comida a ser removida</param>
        public void RemoveFood(Food food)
        {
            this.Clients.Remove(food);
        }

        /// <summary>
        ///     Evento chamado para enviar as posições de todas os clientes dentro do reino
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void SendClientPositions(object sender, ElapsedEventArgs e)
        {
            List<Client> clients = new List<Client>(this.Clients);

            // Obtem todos as celulas conectadas e agrupa pelo usuário (OwnerID)
            foreach(IGrouping<UInt32, Cell> cellGroup in clients.OfType<Cell>().GroupBy(x => x.OwnerID))
            {
                    Double xLeft, xRigth;
                    Double yBottom, yTop;
                    Cell cell = cellGroup.First();      // Primeira celula do grupo, será usada para enviar os dados já que todas as celulas do grupo compartilham do mesmo socket
                    Packet16 packet = new Packet16();

                    // Se o usuário estiver conectado, centraliza nele
                    if (cell.Connected)
                    {
                        xLeft = this.Width;
                        xRigth = this.X;
                        yBottom = this.Height;
                        yTop = this.Y;

                        // Pega as extremidades
                        foreach (Cell c in cellGroup)
                        {
                            if (c.Position.X > xRigth)
                                xRigth = c.Position.X;
                            if (c.Position.X < xLeft)
                                xLeft = c.Position.X;
                            if (c.Position.Y > yTop)
                                yTop = c.Position.Y;
                            if (c.Position.X < yBottom)
                                yBottom = c.Position.Y;
                        }
                    }
                    // Caso contrario, centraliza no meio do mapa (o usuário ainda não clicou em Play)
                    else
                    {
                        yTop = yBottom = xRigth = xLeft = this.Width / 2;
                    }

                    // Pega tudo que está dentro do raio de visão 1200,
                    // futuramente seria bom realizar o calculo pelo tamanho da celula
                    packet.AddClients(clients
                        .Where(c =>
                            c.Position.X < xRigth + 1200 && c.Position.Y < yTop + 1200 &&    // Primeiro quadrante
                            c.Position.X > xLeft - 1200  && c.Position.Y < yTop + 1200 &&    // Segundo quadrante
                            c.Position.X > xLeft - 1200  && c.Position.Y > yBottom - 1200 && // Terceiro quadrante
                            c.Position.X < xRigth + 1200 && c.Position.Y > yBottom - 1200    // Quarto quadrante
                        )
                        .ToArray()
                    );
                    
                    cell.Send(packet.ToByteArray());
                }
            
        }

        private void SendCellRanking(object sender, ElapsedEventArgs e)
        {
            List<Cell> cells = new List<Client>(this.Clients).OfType<Cell>().ToList();

            List<Ranking> ranks = cells
                .Where(cell => cell.Connected)
                .GroupBy(cell => cell.OwnerID)
                .Select(g => new Ranking
                {
                    Id = g.Key,
                    Score = (UInt32)g.Sum(cell => cell.Score),
                    Name = g.Max(cell => cell.Name)
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            Packet49 packet = new Packet49();
            packet.SetRankig(ranks.GetRange(0, ranks.Count < 10 ? ranks.Count : 10));
            this.SendToAllClients(packet.ToByteArray());
        }

        /// <summary>
        ///     Evento chamado para recriar as comidas que foram comidas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateFood(object sender, ElapsedEventArgs e)
        {
            Food food;

            Log.Info(String.Format("The realm {0} will create {1} foods", this.Id, Config.FoodCount - this.FoodCount));

            Parallel.For(this.FoodCount, Config.FoodCount, i =>
            {
                food = new Food();
                food.Position = this.GetRandomPoint();
                this.AddFood(food);
            });

            Log.Info(String.Format("The realm {0} created the foods", this.Id));
        }

        /// <summary>
        ///     Evento chamado para recriar os virus que foram destruidos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateVirus(object sender, ElapsedEventArgs e)
        {
            Virus virus;

            Log.Info(String.Format("The realm {0} will create {1} virus", this.Id, Config.VirusCount - this.VirusCount));

            Parallel.For(this.VirusCount, Config.VirusCount, i =>
            {
                virus = new Virus();
                virus.Position = this.GetRandomPoint();
                this.AddVirus(virus);
            });

            Log.Info(String.Format("The realm {0} created the virus", this.Id));
        }

        /// <summary>
        ///     Envia uma array de bytes para todos os clientes do tipo celula que estão conectados
        /// </summary>
        /// <param name="data">Array de bytes a ser enviado</param>
        public async void SendToAllClients(Byte[] data)
        {
            List<Client> clientes = new List<Client>(this.Clients);
            foreach(Cell cell in clientes.OfType<Cell>())
            {
                cell.Send(data);
            }
        }

        /// <summary>
        ///     Cria um ponto qualquer dentro do reino
        /// </summary>
        /// <returns>Ponto dentro do reino</returns>
        private Point GetRandomPoint()
        {
            return new Point(
                this._rdm.Next(32, (Int32)this.Width - 32), 
                this._rdm.Next(32, (Int32)this.Height - 32)
            );
        }
    }
}
