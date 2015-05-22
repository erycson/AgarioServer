using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game.Packet
{
    /// <summary>
    ///     Envia os dados do mapa
    /// </summary>
    class Packet64 : Packet
    {
        public Packet64() : base(64)
        { }

        public void SetPosition(Double x, Double y)
        {
            this.Writer.Write(x);
            this.Writer.Write(x);
        }

        public void SetSize(Double width, Double height)
        {
            this.Writer.Write(width);
            this.Writer.Write(height);
        }
    }
}
