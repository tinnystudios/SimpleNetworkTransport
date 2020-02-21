using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class ClientConnectionRPC : RPC<int>
    {
        public override int Capacity => 4;
        public override int Id => 0;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            Data = reader.ReadInt(ref context);
        }

        public override void Write(DataStreamWriter writer, int data)
        {
            writer.Write(data);
        }
    }
}