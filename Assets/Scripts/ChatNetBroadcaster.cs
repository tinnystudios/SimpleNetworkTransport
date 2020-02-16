using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetBroadcaster : NetSender
{
    public Text Label;

    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(100000, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        writer.WriteString(Label.text);
        return writer;
    }
}