using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;

namespace ZmqTest
{
    class AudioSink : ProcessNode
    {
        public AudioSink(Context ctx, Barrier bar)
            : base(ctx, bar)
        {
        }

        protected override byte[] Process(byte[][] input)
        {
            return input[0];
        }
    }
}
