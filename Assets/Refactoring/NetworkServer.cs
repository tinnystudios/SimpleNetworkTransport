using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class NetworkServer : NetworkServerBase
    {
        public Action<NetworkConnection> OnClientConnected;
        public Action<int> OnClientDisconnected;

        public List<INetworkReader> Readers;

        protected override void ClientConnected(NetworkConnection connection)
        {
            OnClientConnected?.Invoke(connection);
        }

        protected override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
        {
            var readerId = stream.ReadInt(ref context);

            var reader = Readers.SingleOrDefault(x => x.Id == readerId);
            reader.Read(stream, ref context);
        }
    }
}