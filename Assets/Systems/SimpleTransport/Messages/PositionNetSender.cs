using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class PositionNetSender : NetSender
{
    public bool Log;

    public override DataStreamWriter GetNew() { return new DataStreamWriter(12, Allocator.Temp); }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        if (InstanceId != null)
            writer.Write(InstanceId.Value);

        var val = transform.position.x * 1000;
        writer.Write((int)val);

        return writer;
    }
}
