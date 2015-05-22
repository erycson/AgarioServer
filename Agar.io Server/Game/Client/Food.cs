using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Timers;

namespace AgarioServer.Game.Clients
{
    public class Food : Client
    {
        private Int32 _salts = 0;
        private Vector _direction;
        private Timer _time;

        private Random _rdm;

        public Food()
            : base(ClientType.Food)
        {
            this._rdm = new Random((Int32)this.Id);
            this.Score = 1;
            this.Color = Color.FromKnownColor(Client.Colors[this._rdm.Next(Client.Colors.Length)]);
        }

        public Food(Vector direction)
            : base(ClientType.Food)
        {
            this._direction = direction;

            this._time = new Timer(1000 / 30);
            this._time.Elapsed += MoveFood;
            this._time.Start();
        }

        private void MoveFood(object sender, ElapsedEventArgs e)
        {
            this.Position = this.Position + this._direction * (40 - this._salts);

            List<Virus> clients = new List<Client>(this.Realm.Clients).OfType<Virus>().ToList();

            // Verifica se algum virus vai comer
            Virus virus = clients.AsParallel().FirstOrDefault(v => v.InsideClient(this.Position));
            if (virus != null)
            {
                virus.Score += this.Score;
                this.StopMove();
                this.Realm.RemoveFood(this);

                if (virus.Score > 200)
                    virus.Split(this._direction);

                if (this._time != null)
                    this._time.Stop();
            }
            else
            {
                if (this._salts++ > 10)
                    this._time.Stop();
            }
        }

        public void StopMove()
        {
            if (this._time != null)
            {
                this._time.Stop();
                this._time = null;
            }
        }
    }
}
