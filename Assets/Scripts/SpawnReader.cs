using Unity.Networking.Transport;
using UnityEngine;

public class SpawnReader : NetReader
{
    public Spawner Spawner;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var prefabId = stream.ReadInt(ref context);
        var instanceId = stream.ReadInt(ref context);

        Debug.Log($"Spawn: {prefabId} Instance ID: {instanceId}");

        var client = GetComponentInParent<ClientBehaviour>();
        Spawner.SpawnInClient(prefabId, instanceId, client);
    }
}
