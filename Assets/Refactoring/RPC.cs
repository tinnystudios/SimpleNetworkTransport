using Unity.Collections;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public abstract class RPC<T> : INetworkReader
    {
        public abstract int Capacity { get; }
        public abstract int Id { get; }

        public T Data;

        public DataStreamWriter CreateWriter(T data)
        {
            var writer = new DataStreamWriter(Capacity, Allocator.Temp);
            writer.Write(Id);
            Write(writer, data);

            return writer;
        }

        public abstract void Write(DataStreamWriter writer, T data);
        public abstract void Read(DataStreamReader reader, ref DataStreamReader.Context context);
    }
}