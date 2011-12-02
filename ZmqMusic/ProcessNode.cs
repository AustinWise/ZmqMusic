using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;

namespace ZmqTest
{
    abstract class ProcessNode : Node
    {
        public ProcessNode(Context ctx, Barrier bar)
            : base(ctx, bar)
        {
        }

        protected override byte[][] TryGetInput(Socket[] subs)
        {
            return base.GetInputFromSubs(subs);
        }
    }
}
