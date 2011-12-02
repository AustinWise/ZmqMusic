using System;
using System.Collections.Generic;
using System.Linq;
using ZMQ;
using System.Threading;

namespace ZmqTest.Nodes
{
    class SinNode : GeneratorNode
    {
        private double mFreqMod;
        public SinNode(Context ctx, Barrier bar, double freqMod)
            : base(ctx, bar)
        {
            this.mFreqMod = freqMod;
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
                    var s = Math.Sin(mFreqMod * i);
                    //s += 1;
                    //s /= 2;
                    s = Math.Abs(s);
                    if (i % 1000 == 0)
                    {
                        //Console.WriteLine(s);
                    }
                    sp[i] = (short)(30000 * s);
                }
                //Console.WriteLine(DateTime.Now);
            }
            return ret;
        }
    }
}
