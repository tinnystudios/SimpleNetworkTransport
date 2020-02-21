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
            
            var reader = Readers.SingleOrDefault(x => x.Id == readerId);

            if (reader.ConnectionId != null)
                stream.ReadInt(ref context);

            reader.Read(stream, ref context);
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