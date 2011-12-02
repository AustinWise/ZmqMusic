The idea here is you create a directed, acyclic graph to represent the flow
of audio bytes.  Nodes with no parents have to generate audio with no
input, while other nodes get all their parent's output as input.  One
node's output is fed to to the sound device.

This project is for me to fiddle around with audio a bit and also play a
little with ZMQ.  There are many things that could be better (like
less absurd byte copying).
