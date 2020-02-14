using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetSender : NetSender
{
    public InputField InputField;
    public ClientBehaviour Client;

    public override DataStreamWriter Write()
    {
        var writer = new DataStreamWriter(100000, Allocator.Temp);
        writer.WriteString(InputField.text);

        return writer;
    }

    public void Send()
    {
        Client.Send(Write());
    }
}