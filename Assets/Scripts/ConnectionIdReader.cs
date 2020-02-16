using Unity.Networking.Transport;

public class ConnectionIdReader : NetReader
{
    public int ServerAssignedConnectionId = -1;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        ServerAssignedConnectionId = stream.ReadInt(ref context);
    }
}
