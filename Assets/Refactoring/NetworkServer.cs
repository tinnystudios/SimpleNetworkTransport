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

        public NativeList<NetworkConnection> Connections => m_Connections;

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
            var readerId = stream.ReadInt(ref context);
            //Debug.Log($"Reading: {readerId}");

            foreach (var reader in Readers)
            {
                if (reader.Id != readerId)
                    continue;

                if (reader.ConnectionId != null)
                {
                    var conId = stream.ReadInt(ref context);
                }

                if (reader.InstanceId != null)
                {
                    var instanceId = stream.ReadInt(ref context);
                    if (reader.InstanceId != instanceId)
                        continue;
                }

                reader.Read(stream, ref context);
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
    }
}