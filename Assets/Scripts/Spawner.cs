using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Server Server;
    public Ghost GhostPrefab;

    private void Start()
    {
        var instance = Instantiate(GhostPrefab);

        var readers = instance.GetComponentsInChildren<NetReader>();
        var senders = instance.GetComponentsInChildren<NetSender>();

        foreach (var c in Server.Connections)
        {
            var writer = new DataStreamWriter(1000, Allocator.Temp);
            writer.Write(10); // Unique Packet ID
            writer.Write(1);  // Prefab ID
            writer.Write(instance.GetInstanceID());
            Server.Write(writer, c);
        }

        /*
        foreach (var sender in senders)
            Server.AddSender(sender);
        */
    }
}

public class SpawnReader : NetReader
{
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var prefabId = stream.ReadInt(ref context);
        var instanceId = stream.ReadInt(ref context);
        // Create an object from the prefab id. Collection[prefabId]
    }
}