using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgarioServer.Game.Packet
{

    // 16 - Movimentação
    // 48 ou 49 - Ranking
    // 64 - Move a camera
    class Packet
    {
        private MemoryStream _stream;
        protected BinaryWriter Writer;

        public Packet(Byte type)
        {
            this._stream = new MemoryStream();
            this.Writer = new BinaryWriter(_stream);
            this.Writer.Write(type);
        }

        public Byte[] ToByteArray()
        {
            using (BinaryReader reader = new BinaryReader(_stream))
            {
                _stream.Position = 0;
                return reader.ReadBytes((int)_stream.Length);
            }
        }
    }
}
