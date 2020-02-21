using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class NetworkServer : NetworkServerBase
    {
        public List<INetworkReader> Readers;

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
            var reader = Readers.SingleOrDefault(x => x.Id == readerId);
            reader.Read(stream, ref context);
        }

        private void CleanReaders(int connectionId)
        {
            for (int i = 0; i < Readers.Count; i++)
            {
                if(Readers[i])
            }
        }
    }
}