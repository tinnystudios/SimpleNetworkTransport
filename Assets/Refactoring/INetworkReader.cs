using Unity.Networking.Transport;

namespace SimpleTransport
{
    public interface INetworkReader
    {
        void Read(DataStreamReader reader, ref DataStreamReader.Context context);
        int Id { get; }
        int ConnectionId { get; }
        int InstanceId { get; }
    }
}