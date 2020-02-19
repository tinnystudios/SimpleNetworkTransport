using Unity.Networking.Transport;
using Unity.Collections;

/// <summary>
/// Send the ID allocated by the server for a connection back to the connection.
/// </summary>
public class ConnectionIdSender : NetSender
{
    public Server Server;

    private void Awake()
    {
        Server.OnNewConnection += OnNewConnection;
    }

    private void OnNewConnection(NetworkConnection connection)
    {
        var writer = GetNew();
        writer.Write(Id);
        writer.Write(connection.InternalId);
        Server.Write(writer, connection);
    }

    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(8, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter stream)
    {
        return GetNew();
    }
}
