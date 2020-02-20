using System;
using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetReader : NetReader
{
    public Transform Target;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        byte[] buff = stream.ReadBytesAsArray(ref context, 12);
        Vector3 vect = Vector3.zero;

        vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));

        Target.position = vect;
    }
}
