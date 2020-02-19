using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetSender : NetSender
{
    public InputField InputField;
    public ClientBehaviour Client;

    public override DataStreamWriter GetNew()
    {
        return new DataStreamWriter(100000, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        writer.WriteString(InputField.text);
        return writer;
    }

    public void Send()
    {
        var writer = GetNew();
        writer.Write(Id);
        Write(writer);
        Client.Send(writer);
    }
}
