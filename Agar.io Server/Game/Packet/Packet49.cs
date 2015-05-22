using AgarioServer.Game.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game.Packet
{
    /// <summary>
    ///     Envia o Ranking do reino
    /// </summary>
    class Packet49 : Packet
    {
        private List<Ranking> _rank;

        public Packet49() : base(49)
        { }

        public void SetRankig(List<Ranking> rank)
        {
            this._rank = rank;
        }
        public Byte[] ToByteArray()
        {
            // Destruir
            this.Writer.Write((UInt32)this._rank.Count);

            foreach (Ranking rank in this._rank)
            {
                this.Writer.Write(rank.Id);
                if (rank.Name != null)
                    this.Writer.Write(rank.Name);
                this.Writer.Write((UInt16)0);
            }

            return base.ToByteArray();
        }
    }
}
