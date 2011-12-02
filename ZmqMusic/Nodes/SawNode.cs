using System;
using System.Collections.Generic;
using System.Linq;
using ZMQ;
using System.Threading;

namespace ZmqTest.Nodes
{
    class SawNode : GeneratorNode
    {
        public SawNode(Context ctx, Barrier bar)
            : base(ctx, bar)
        {
        }

        unsafe protected override byte[] Process(byte[][] input)
        {
            var size = Program.SampleSize;
            var ret = new byte[size];
            fixed (byte* bp = ret)
            {
                short* sp = (short*)bp;
                size /= 2;
                for (int i = 0; i < size; i++)
                {
                    sp[i] = (short)(i * 500);
                }
            }
            return ret;
        }
    }
}
