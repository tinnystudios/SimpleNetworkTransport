using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public abstract class RPC<T> : INetworkReader, INetworkWriter
    {
        public int BaseCapacity => 4;
        public abstract int Capacity { get; }
        public abstract int Id { get; }

        public int? ConnectionId { get; set; }
        public int? InstanceId { get; set; }

        public T Data;

        public DataStreamWriter CreateWriter(T data, int? connectionId = null, int? instanceId = null)
        {
            ConnectionId = connectionId;
            InstanceId = instanceId;
            Data = data;

            var totalCapacity = BaseCapacity;
            totalCapacity += connectionId == null ? 0 : 4;
            totalCapacity += connectionId == null ? 0 : 4;
            totalCapacity += Capacity;

            var writer = new DataStreamWriter(totalCapacity, Allocator.Temp);
            writer.Write(Id);

            if (connectionId != null)
                writer.Write(connectionId.Value);

            if (instanceId != null)
                writer.Write(instanceId.Value);

            Write(writer, data);

            return writer;
        }

        public override string ToString() { return $"RPC Data: {Data}"; }

        public abstract void Write(DataStreamWriter writer, T data);
        public abstract void Read(DataStreamReader reader, ref DataStreamReader.Context context);

        public DataStreamWriter Write(int? connectionId = null, int? instanceId = null)
        {
            var writer = CreateWriter(Data, ConnectionId, InstanceId);
            return writer;
        }
    }
}