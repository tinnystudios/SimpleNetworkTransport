using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetReader : NetReader
{
    public Transform Target;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var val = stream.ReadString(ref context);
        var array = val.ToString().Split(',');
        Target.position = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
    }
}
