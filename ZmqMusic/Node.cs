using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZMQ;

namespace ZmqTest
{
    abstract class Node
    {
        protected const int TIMEOUT = 100;

        public Node(Context ctx, Barrier bar)
        {
            Id = Guid.NewGuid();
            mParents = new List<Node>();

            mBarrier = bar;
            mContext = ctx;
        }

        public Guid Id { get; private set; }
        public String Url { get { return "inproc://" + Id; } }

        private List<Node> mParents;
        private bool mIsRunning = true;
        private Barrier mBarrier;
        private Context mContext;

        public void AddParent(Node node)
        {
            if (node == null)
                throw new ArgumentNullException();
            mParents.Add(node);
        }

        public void Start()
        {
            new Thread(WorkThread).Start();
        }

        public void Stop()
        {
            mIsRunning = false;
        }

        protected abstract byte[] Process(byte[][] input);

        protected abstract byte[][] TryGetInput(Socket[] subs);

        private void WorkThread()
        {
            var parents = mParents.ToArray();
            var subs = new Socket[parents.Length];

            var pub = mContext.Socket(SocketType.PUB);
            pub.Bind(Url);

            mBarrier.SignalAndWait();

            for (int i = 0; i < parents.Length; i++)
            {
                var sub = mContext.Socket(SocketType.SUB);
                sub.Subscribe(new byte[0]);
                sub.Connect(parents[i].Url);
                subs[i] = sub;
            }

            mBarrier.SignalAndWait();

            while (mIsRunning)
            {
                byte[][] bytes = TryGetInput(subs);
                if (bytes == null)
                {
                    continue;
                }

                var ret = Process(bytes);
                pub.Send(ret);
            }

            mBarrier.SignalAndWait();

            pub.Dispose();
            for (int i = 0; i < parents.Length; i++)
            {
                subs[i].Dispose();
            }
        }


        //function library
        protected byte[][] GetInputFromSubs(Socket[] subs)
        {
            var polls = subs.Select(s => s.CreatePollItem(IOMultiPlex.POLLIN)).ToArray();
            var hit = mContext.Poll(polls, TIMEOUT);
            if (hit != subs.Length)
                return null;

            byte[][] bytes = new byte[subs.Length][];

            for (int i = 0; i < subs.Length; i++)
            {
                bytes[i] = subs[i].Recv();
            }

            return bytes;
        }
    }
}
