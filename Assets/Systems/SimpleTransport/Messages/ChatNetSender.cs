using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetSender : NetSender
{
    public InputField InputField;
    public ClientBehaviour Client;

    public override DataStreamWriter GetNew()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(InputField.text);
        return new DataStreamWriter(bytes.Length + 8, Allocator.Temp);
    }

    public override DataStreamWriter Write(DataStreamWriter writer)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(InputField.text);
        writer.Write(bytes.Length);
        writer.Write(bytes);
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
