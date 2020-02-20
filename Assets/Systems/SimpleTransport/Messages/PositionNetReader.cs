using System;
using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetReader : NetReader
{
    public Transform Target;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        byte[] buff = stream.ReadBytesAsArray(ref context, sizeof(float) * 7);
        Vector3 vect = Vector3.zero;

        vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));

        var rotation = Quaternion.identity;
        rotation.x = BitConverter.ToSingle(buff, 3 * sizeof(float));
        rotation.y = BitConverter.ToSingle(buff, 4 * sizeof(float));
        rotation.z = BitConverter.ToSingle(buff, 5 * sizeof(float));
        rotation.w = BitConverter.ToSingle(buff, 6 * sizeof(float));

        Target.rotation = rotation;
        Target.position = vect;
    }
}
