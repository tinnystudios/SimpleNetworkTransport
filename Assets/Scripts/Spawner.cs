using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Server Server;

    // The prefab as a pair
    public Ghost GhostPrefab;
    public Ghost Prefab;

    public Transform SpawnPoint;

    public const int SpawnId = 10;

    [ContextMenu("Spawn Test")]
    public void SpawnTest()
    {
        SpawnInServer(9999);
    }

    public void SpawnInServer(int prefabId)
    {
        var instance = Instantiate(Prefab, transform);
        var instanceId = instance.GetInstanceID();

        instance.transform.position = SpawnPoint.position;

        // The sender will automatically handle the positioning? 
        foreach (var sender in instance.Senders)
        {
            sender.InstanceId = instanceId;
            Server.AddSender(sender);
        }

        // If you are the server, notify all clients that a new object has been made
        foreach (var c in Server.Connections)
        {
            var writer = new DataStreamWriter(1000000, Allocator.Temp);
            writer.Write(SpawnId);
            writer.Write(prefabId);
            writer.Write(instanceId);
            Server.Write(writer, c);
        }
    }

    public void SpawnInClient(int prefabId, int instanceId, ClientBehaviour client)
    {
        var instance = Instantiate(GhostPrefab, client.transform);
        foreach (var reader in instance.Readers)
        {
            reader.InstanceId = instanceId;
            client.Readers.Add(reader);
        }
    }
}
