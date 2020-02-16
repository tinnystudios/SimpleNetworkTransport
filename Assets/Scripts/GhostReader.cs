using Unity.Networking.Transport;
using UnityEngine;

/// <summary>
/// This is read when a connection you do not know of enters.
/// </summary>
public class GhostReader : NetReader
{
    public GhostCollection GhostCollection;
    public ConnectionIdReader ConnectionIdReader;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var str = stream.ReadString(ref context);
        var client = GetComponentInParent<ClientBehaviour>();

        if (string.IsNullOrEmpty(str.ToString()))
            return;

        var array = str.ToString().Split(',');

        foreach (var val in array)
        {
            var id = int.Parse(val);

            if (id == ConnectionIdReader.ServerAssignedConnectionId)
                return;

            Debug.Log($"{client.name} added ghost: {id}");

            GhostCollection.NewGhost(client, id);
        }
    }
}
