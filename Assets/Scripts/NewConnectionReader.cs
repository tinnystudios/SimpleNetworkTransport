using Unity.Networking.Transport;

/// <summary>
/// This is read when a connection you do not know of enters.
/// </summary>
public class NewConnectionReader : NetReader
{
    public GhostCollection GhostCollection;
    public ConnectionIdReader ConnectionIdReader;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var str = stream.ReadString(ref context);

        if (string.IsNullOrEmpty(str.ToString()))
            return;

        var array = str.ToString().Split(',');

        // How to ignore yourself?
        // You first need to know your connection id...

        foreach (var val in array)
        {
            var id = int.Parse(val);

            if (id == ConnectionIdReader.ServerAssignedConnectionId)
                return;

            var client = GetComponentInParent<ClientBehaviour>();
            GhostCollection.NewGhost(client, id);
        }
    }
}
