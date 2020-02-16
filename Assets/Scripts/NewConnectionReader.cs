using Unity.Networking.Transport;

/// <summary>
/// This is read when a connection you do not know of enters.
/// </summary>
public class NewConnectionReader : NetReader
{
    public GhostCollection GhostCollection;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var str = stream.ReadString(ref context);

        if (string.IsNullOrEmpty(str.ToString()))
            return;

        var array = str.ToString().Split(',');

        foreach (var val in array)
        {
            var id = int.Parse(val);
            var client = GetComponentInParent<ClientBehaviour>();

            GhostCollection.NewGhost(client, id);
        }
    }
}
