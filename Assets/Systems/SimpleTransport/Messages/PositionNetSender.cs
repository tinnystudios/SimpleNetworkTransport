using Unity.Collections;
using Unity.Networking.Transport;

public class PositionNetSender : NetSender
{
    public override DataStreamWriter GetNew() { return new DataStreamWriter(20, Allocator.Temp); }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        if (InstanceId != null)
            writer.Write(InstanceId.Value);

        var x = transform.position.x * 10000;
        var y = transform.position.y * 10000;
        var z = transform.position.z * 10000;

        writer.Write((int)x);
        writer.Write((int)y);
        writer.Write((int)z);

        return writer;
    }
}
