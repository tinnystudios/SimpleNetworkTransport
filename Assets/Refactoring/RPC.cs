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

        public INetwork Network { get; set; }

        public T Data;

        public ReaderHeader Match(DataStreamReader stream, ref DataStreamReader.Context context)
        {
            var readerId = stream.ReadInt(ref context);
            if (Id != readerId)
                return new ReaderHeader(false);

            var conId = -1;
            var instanceId = -1;

            if (ConnectionId != null)
            {
                conId = stream.ReadInt(ref context);
            }

            if (InstanceId != null)
            {
                instanceId = stream.ReadInt(ref context);
                if (InstanceId != instanceId)
                    return new ReaderHeader(false);
            }

            return new ReaderHeader
            {
                Matched = true,
                InstanceId = instanceId,
                ConnectionId = conId,
            };
        }

        public DataStreamWriter CreateWriter(T data, INetwork network, int? connectionId = null, int? instanceId = null)
        {
            ConnectionId = connectionId;
            InstanceId = instanceId;
            Data = data;

            var totalCapacity = BaseCapacity;
            totalCapacity += connectionId == null ? 0 : 4;
            totalCapacity += instanceId == null ? 0 : 4;
            totalCapacity += Capacity;

            var writer = new DataStreamWriter(totalCapacity, Allocator.Temp);
            writer.Write(Id);
           
            if (connectionId != null)
            {
                writer.Write(connectionId.Value);
            }
            if (instanceId != null)
            {
                writer.Write(instanceId.Value);
            }

            // TODO Befoer you even implement the tick, make sure to move the 'read' here.
            //writer.Write(network.CurrentTick);

            Write(writer, data);

            return writer;
        }

        public override string ToString() { return $"RPC Data: {Data}"; }

        public abstract void Write(DataStreamWriter writer, T data);
        public abstract void Read(DataStreamReader reader, ref DataStreamReader.Context context);

        public DataStreamWriter Write(INetwork network, int? connectionId = null, int? instanceId = null)
        {
            var writer = CreateWriter(Data, network, ConnectionId, InstanceId);
            return writer;
        }
    }

    public interface INetworkUpdate 
    {
        void Update();
    }

    public interface INetwork
    {
        int CurrentTick { get; }
    }

    public struct ReaderHeader
    {
        public bool Matched;
        public int ConnectionId;
        public int InstanceId;

        public ReaderHeader(bool matched)
        {
            Matched = false;
            ConnectionId = -1;
            InstanceId = -1;
        }
    }
}