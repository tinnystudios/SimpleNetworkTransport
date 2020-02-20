using Unity.Networking.Transport;
using UnityEngine;

public class DespawnReader : NetReader
{
    public Spawner Spawner;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var instanceId = stream.ReadInt(ref context);
        Debug.Log(instanceId + " to be despawned");
        Spawner.DespawnInClient(instanceId, GetComponentInParent<Client>());
    }
}