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
        var client = GetComponentInParent<ClientBehaviour>();
        var length = stream.ReadInt(ref context);

        Debug.Log($"Length: {length}");

        for (int i = 0; i < length; i++)
        {
            var prefabId = stream.ReadInt(ref context);
            var instanceId = stream.ReadInt(ref context);

            GhostCollection.NewInClient(prefabId, instanceId, (int)EOwnershipType.Server, client);
        }
    }
}
