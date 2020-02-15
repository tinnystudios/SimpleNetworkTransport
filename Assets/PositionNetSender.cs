﻿using Unity.Collections;
using Unity.Networking.Transport;

public class PositionNetSender : NetSender
{
    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(1000, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        writer.WriteString($"{transform.position.x},{transform.position.y},{transform.position.z}");
        return writer;
    }
}
