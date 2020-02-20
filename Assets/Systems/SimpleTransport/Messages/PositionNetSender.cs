using System;
using Unity.Collections;
using Unity.Networking.Transport;

public class PositionNetSender : NetSender
{
    public override DataStreamWriter GetNew() { return new DataStreamWriter(20, Allocator.Temp); }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        if (InstanceId != null)
            writer.Write(InstanceId.Value);

        byte[] buff = new byte[sizeof(float) * 3];
        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.x), 0, buff, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.y), 0, buff, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.z), 0, buff, 2 * sizeof(float), sizeof(float));

        writer.Write(buff);

        return writer;
    }
}
