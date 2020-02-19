using Unity.Collections;
using Unity.Networking.Transport;

public class PingSender : NetSender
{
    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(8, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        writer.Write(0);
        return writer;
    }
}