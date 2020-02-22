using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace SimpleTransport
{
    public class NetworkServer : NetworkServerBase
    {
        public List<INetworkReader> Readers = new List<INetworkReader>();
        public List<INetworkWriter> Writers = new List<INetworkWriter>();

        public NativeList<NetworkConnection> Connections => m_Connections;

        protected override void OnUpdate()
        {
            foreach (var connection in m_Connections)
            {
                foreach (var writer in Writers)
                {
                    var streamWriter = writer.Write();
                    Write(streamWriter, connection);
                }
            }
        }

        protected override void ClientConnected(NetworkConnection connection)
        {
            var writer = new ClientConnectionRPC().CreateWriter(connection.InternalId);
            Write(writer, connection);
        }

        protected override void ClientDisconnected(int id)
        {
            CleanReaders(id);
        }

        protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
        {
            foreach (var reader in Readers)
            {
                var c = default(DataStreamReader.Context);
                var readerId = stream.ReadInt(ref c);

                if (reader.Id != readerId)
                    continue;

                if (reader.ConnectionId != null)
                {
                    var conId = stream.ReadInt(ref c);
                }

                if (reader.InstanceId != null)
                {
                    var instanceId = stream.ReadInt(ref c);
                    if (reader.InstanceId != instanceId)
                        continue;
                }

                reader.Read(stream, ref c);
            }
        }

        private void CleanReaders(int connectionId)
        {
            for (var i = 0; i < Readers.Count; i++)
            {
                if (Readers[i].ConnectionId != connectionId)
                    continue;

                Readers.RemoveAt(i);
                i--;
            }
        }

        public void AddReader(INetworkReader reader)
        {
            Readers.Add(reader);
        }

        public void AddWriter(INetworkWriter writer)
        {
            Writers.Add(writer);
        }
    }
}