using Unity.Collections;
using Unity.Networking.Transport;

public class PingSender : NetSender
{
    public override DataStreamWriter Write()
    {
        var writer = new DataStreamWriter(4, Allocator.Temp);
        writer.Write(0);
        return writer;
    }
}