using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetReader : NetReader
{
    public Transform Target;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var x = (float)stream.ReadInt(ref context)/1000;
        var pos = Target.position;
        pos.x = x;

        Target.position = pos;
    }
}
