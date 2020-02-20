using System;
using Unity.Networking.Transport;
using UnityEngine;

public class SpawnRequestReader : NetReader
{
    public Spawner Spawner;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var prefabId = stream.ReadInt(ref context);

        byte[] buff = stream.ReadBytesAsArray(ref context, sizeof(float) * 7);
        Vector3 position = Vector3.zero;

        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        position.z = BitConverter.ToSingle(buff, 2 * sizeof(float));

        var rotation = Quaternion.identity;
        rotation.x = BitConverter.ToSingle(buff, 3 * sizeof(float));
        rotation.y = BitConverter.ToSingle(buff, 4 * sizeof(float));
        rotation.z = BitConverter.ToSingle(buff, 5 * sizeof(float));
        rotation.w = BitConverter.ToSingle(buff, 6 * sizeof(float));

        Spawner.SpawnInServer(prefabId, position, rotation);
    }
}
