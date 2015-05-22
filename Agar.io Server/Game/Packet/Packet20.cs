using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game.Packet
{
    /// <summary>
    ///     Tira o player do jogo
    /// </summary>
    class Packet20 : Packet
    {
        public Packet20() : base(20)
        { }
    }
}
