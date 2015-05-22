using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace AgarioServer.Game.Clients
{
    public class Virus : Client
    {
        private Timer _time;
        private Int32 _salts = 0;
        private Vector _direction;

        public Virus() : base(ClientType.Virus)
        {
            this.Score = 100;
            this.Color = Color.FromArgb(51, 251, 51);
        }

        public Virus(System.Windows.Point position, Vector direction)
            : this()
        {
            this.Position = position;
            this._direction = direction;

            this._time = new Timer(1000 / 30);
            this._time.Elapsed += MoveVirus;
            this._time.Start();
        }

        public void Split(Vector direction)
        {
            this.Score = 100;
            this.Realm.AddVirus(new Virus(this.Position, direction), true);
        }

        private void MoveVirus(object sender, ElapsedEventArgs e)
        {
            this.Position = this.Position + this._direction * (40 - this._salts);

            if (this._salts++ > 13)
                this._time.Stop();
        }
    }
}
