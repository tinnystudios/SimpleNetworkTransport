using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class NetworkClient : NetworkClientBase
    {
        public List<INetworkReader> Readers;

        private void Awake()
        {
            // Read on connected
        }

        protected override void Read(DataStreamReader stream)
        {
            var context = default(DataStreamReader.Context);
            var readerId = stream.ReadInt(ref context);

            var reader = Readers.SingleOrDefault(x => x.Id == readerId);
            reader.Read(stream, ref context);
        }

        protected override void Write(DataStreamWriter writer, UdpNetworkDriver driver, NetworkConnection connection)
        {
            connection.Send(driver, writer);
            writer.Dispose();
        }
    }
}