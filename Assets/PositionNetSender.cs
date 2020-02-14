using Unity.Collections;
using Unity.Networking.Transport;

public class PositionNetSender : NetSender
{
    public override DataStreamWriter Write()
    {
        var writer = new DataStreamWriter(1000, Allocator.Temp);
        writer.WriteString($"{transform.position.x},{transform.position.y},{transform.position.z}");
        return writer;
    }
}
