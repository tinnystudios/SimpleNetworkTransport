using Unity.Networking.Transport;
using UnityEngine;

public class SpawnRequestReader : NetReader
{
    public Spawner Spawner;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var prefabId = stream.ReadInt(ref context);

        var position = Vector3.one;
        position.x = stream.ReadFloat(ref context);
        position.y = stream.ReadFloat(ref context);
        position.z = stream.ReadFloat(ref context);

        var rotation = Quaternion.identity;
        rotation.x = stream.ReadFloat(ref context);
        rotation.y = stream.ReadFloat(ref context);
        rotation.z = stream.ReadFloat(ref context);
        rotation.w = stream.ReadFloat(ref context);

        Debug.Log(position);

        Spawner.SpawnInServer(prefabId, position, rotation);
    }
}