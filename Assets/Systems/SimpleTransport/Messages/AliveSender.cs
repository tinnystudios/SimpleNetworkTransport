using Unity.Networking.Transport;

public class AliveSender : NetSender
{
    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(12, Unity.Collections.Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter stream)
    {
        stream.Write(1);
        return stream;
    }
}