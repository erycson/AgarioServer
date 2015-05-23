using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game.Packet
{
    /// <summary>
    ///     Envia os ID da celula do jogador
    /// </summary>
    class Packet32 : Packet
    {
        public Packet32() : base(32)
        { }

        public Packet32 SetId(UInt32 id)
        {
            this.Writer.Write(id);
            return this;
        }
    }
}
