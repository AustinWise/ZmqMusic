using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;

namespace ZmqTest
{
    abstract class GeneratorNode : Node
    {
        private readonly AutoResetEvent mGenEvent;

        public GeneratorNode(Context ctx, Barrier bar)
            : base(ctx, bar)
        {
            mGenEvent = new AutoResetEvent(false);
        }

        public void SignalForData()
        {
            mGenEvent.Set();
        }

        protected override byte[][] TryGetInput(ZMQ.Socket[] subs)
        {
            return mGenEvent.WaitOne(TIMEOUT) ? new byte[0][] : null;
        }
    }
}
