using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class DespawnRPC : RPC<int>
    {
        public override int Capacity => 4;
        public override int Id => 8;

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            var despawnInstance = reader.ReadInt(ref context);
            Logger.Log($"Despawn request for {despawnInstance} received");
        }

        public override void Write(DataStreamWriter writer, int data)
        {
            writer.Write(data);
        }
    }
}