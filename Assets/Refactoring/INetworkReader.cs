using Unity.Networking.Transport;

namespace SimpleTransport
{
    public interface INetworkReader
    {
        ReaderHeader Match(DataStreamReader stream, ref DataStreamReader.Context context);
        void Read(DataStreamReader reader, ref DataStreamReader.Context context);
        int Id { get; }
        int? ConnectionId { get; }
        int? InstanceId { get; }
    }
}