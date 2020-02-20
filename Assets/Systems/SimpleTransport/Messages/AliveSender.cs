using Unity.Networking.Transport;

public class AliveSender : NetSender
{
    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(8, Unity.Collections.Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter stream)
    {
        var writer = GetNew();
        writer.Write(1);
        return writer;
    }
}