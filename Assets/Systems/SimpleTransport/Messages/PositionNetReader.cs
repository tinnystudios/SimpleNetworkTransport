using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetReader : NetReader
{
    public Transform Target;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var x = (float)stream.ReadInt(ref context)/10000;
        var y = (float)stream.ReadInt(ref context)/10000;
        var z = (float)stream.ReadInt(ref context)/10000;

        var pos = Target.position;
        pos.x = x;
        pos.y = y;
        pos.z = z;

        Target.position = Vector3.Lerp(Target.position, pos, 8 * Time.deltaTime);
    }
}
