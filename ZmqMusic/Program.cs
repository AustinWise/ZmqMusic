using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ZMQ;
using ZMQ.ZMQExt;
using ZmqTest.Nodes;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace ZmqTest
{
    static class Program
    {
        public static int SampleSize;
        private const int SampleTimeMs = 5000;

        private static bool isRunning = true;
        private static Barrier mBar = new Barrier(1);
        private static List<Node> mNodes = new List<Node>();

        private static void AddNode(Node n)
        {
            mNodes.Add(n);
            mBar.AddParticipant();
        }

        static void Main(string[] args)
        {
            using (var ctx = new Context(1))
            {
                var gen1 = new SinNode(ctx, mBar, 0.09);
                AddNode(gen1);
                var gen2 = new SinNode(ctx, mBar, 0.1);
                AddNode(gen2);

                var add = new MaxNode(ctx, mBar);
                add.AddParent(gen1);
                add.AddParent(gen2);
                AddNode(add);

                foreach (var n in mNodes)
                {
                    n.Start();
                }

                mBar.SignalAndWait();

                PlayThings(ctx, add);

                foreach (var n in mNodes)
                {
                    n.Stop();
                }

                mBar.SignalAndWait();
            }

            mBar.Dispose();
        }

        private static void PlayThings(Context ctx, Node srcNode)
        {
            FrameworkDispatcher.Update();
            using (var aud = new DynamicSoundEffectInstance(48000, AudioChannels.Mono))
            {
                SampleSize = aud.GetSampleSizeInBytes(new TimeSpan(0, 0, 0, 0, SampleTimeMs));

                var sub = ctx.Socket(SocketType.SUB);
                sub.Subscribe(new byte[0]);
                sub.Connect(srcNode.Url);

                mBar.SignalAndWait();

                bool bufferNeeded = true;
                aud.BufferNeeded += (_, __) => bufferNeeded = true;

                while (isRunning)
                {
                    if (bufferNeeded)
                    {
                        foreach (var n in mNodes.Where(n => n is GeneratorNode).Cast<GeneratorNode>())
                        {
                            n.SignalForData();
                        }
                        bufferNeeded = false;
                    }

                    var bytes = sub.Recv(SampleTimeMs / 2);

                    if (bytes != null)
                    {
                        aud.SubmitBuffer(bytes);
                        if (aud.State == SoundState.Stopped)
                            aud.Play();
                    }

                    FrameworkDispatcher.Update();

                    //running = false;
                }

                aud.Stop();
                sub.Dispose();
            }
        }
    }
}
