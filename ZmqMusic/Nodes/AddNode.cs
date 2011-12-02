using System;
using System.Collections.Generic;
using System.Linq;
using ZMQ;
using System.Threading;

namespace ZmqTest.Nodes
{
    class AddNode : ProcessNode
    {
        public AddNode(Context ctx, Barrier bar)
            : base(ctx, bar)
        {
        }

        unsafe protected override byte[] Process(byte[][] input)
        {
            var size = input[0].Length;
            var ret = new byte[size];
            size /= 2;
            fixed (byte* bp = ret)
            {
                var retP = (short*)bp;
                for (int listNdx = 0; listNdx < input.Length; listNdx++)
                {
                    fixed (byte* otherP = input[listNdx])
                    {
                        var sp = (short*)otherP;
                        for (int i = 0; i < size; i++)
                        {
                            retP[i] += sp[i];
                        }
                    }
                }
            }
            return ret;
        }
    }
}
