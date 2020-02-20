using Unity.Networking.Transport;
using UnityEngine;

public class SpawnRequestReader : NetReader
{
    public Spawner Spawner;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var prefabId = stream.ReadInt(ref context);

        var position = Vector3.one;
        position.x = (float)stream.ReadInt(ref context)/10000;
        position.y = (float)stream.ReadInt(ref context)/10000;
        position.z = (float)stream.ReadInt(ref context)/10000;

        var rotation = Quaternion.identity;
        rotation.y = (float)stream.ReadInt(ref context)/10000;
        rotation.z = (float)stream.ReadInt(ref context)/10000;
        rotation.x = (float)stream.ReadInt(ref context)/10000;
        rotation.w = (float)stream.ReadInt(ref context)/10000;

        Spawner.SpawnInServer(prefabId, position, rotation);
    }
}