using Unity.Networking.Transport;
using UnityEngine;

public class NewConnectionReader : NetReader
{
    public GhostCollection GhostCollection;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var id = stream.ReadInt(ref context);
        var client = GetComponentInParent<ClientBehaviour>();
        GhostCollection.NewGhost(client, id);
    }
}
