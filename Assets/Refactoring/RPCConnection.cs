using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class RPCConnection : RPC<int>
    {
        public override int Capacity => 8;
        public override int Id => 0;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(DataStreamWriter writer, int data)
        {
            throw new System.NotImplementedException();
        }
    }
}