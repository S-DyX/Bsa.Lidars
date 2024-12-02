using System;

namespace Bsa.Velodyne.Entities
{
    public class Packet
    {
        public VerticalBlockPair[] Blocks { get; private set; }
        public uint _Time { get; private set; }
        public VelodyneModel _VelodyneModel { get; private set; }
        public ReturnType _ReturnType { get; private set; }

      

        internal unsafe Packet(Byte* b, out bool valid)
        {
            RawPacket r = *(RawPacket*)b;

            this._Time = 0;
            for (int i = 4; i > 0; i--)
                this._Time = (this._Time << 8) | r._TimeStamp[i - 1];

            this._ReturnType = DataMapper.ReturnType_(r._Factory[0]);
            this._VelodyneModel = DataMapper.VelodyneModel_(r._Factory[1]);

            valid = true;

            this.Blocks = new VerticalBlockPair[RawPacket._VerticalBlockPairsPerPacket];

            for (int i = 0, byte_index = 0;
                i < RawPacket._VerticalBlockPairsPerPacket;
                i++, byte_index += VerticalBlockPair.RawBlockPair._Size)
            {
                bool bb;
                this.Blocks[i] = new VerticalBlockPair(&r._DataBlocks[byte_index], out bb);
                valid &= bb;
            }
        }

      
    }
}
