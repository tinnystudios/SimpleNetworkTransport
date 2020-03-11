using Unity.Networking.Transport;

namespace SimpleTransport
{
    public interface INetworkWriter
    {
        DataStreamWriter Write(INetwork network, int? connectionId = null, int? instanceId = null);
        int? ConnectionId { get; }
        int? InstanceId { get; }
    }
}