using Unity.Collections;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public abstract class RPC<T> : INetworkReader
    {
        public int BaseCapacity => 4;
        public abstract int Capacity { get; }
        public abstract int Id { get; }

        public int? ConnectionId { get; }
        public int? InstanceId { get; }

        public T Data;

        public DataStreamWriter CreateWriter(T data, int? connectionId = null, int? instanceId = null)
        {
            var writer = new DataStreamWriter(BaseCapacity + Capacity, Allocator.Temp);

            if (connectionId != null)
                writer.Write(connectionId.Value);

            if (instanceId != null)
                writer.Write(instanceId.Value);

            writer.Write(Id);
            Write(writer, data);

            return writer;
        }

        public override string ToString() { return $"RPC Data: {Data}"; }

        public abstract void Write(DataStreamWriter writer, T data);
        public abstract void Read(DataStreamReader reader, ref DataStreamReader.Context context);
    }
}