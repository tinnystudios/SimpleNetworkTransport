using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class NetworkClient : NetworkClientBase
    {
        public List<INetworkReader> Readers;
        public ClientConnectionRPC ClientConnectionRpc = new ClientConnectionRPC();

        public int ConnectionId => ClientConnectionRpc.Data;

        private void Awake()
        {
            Readers.Add(ClientConnectionRpc);
        }

        protected override void Read(DataStreamReader stream)
        {
            var context = default(DataStreamReader.Context);
            var readerId = stream.ReadInt(ref context);

            var reader = Readers.SingleOrDefault(x => x.Id == readerId);
            reader.Read(stream, ref context);
        }

        public override void Write(DataStreamWriter writer)
        {
            Connection.Send(Driver, writer);
            writer.Dispose();
        }
    }
}