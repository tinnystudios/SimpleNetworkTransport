using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetReader : NetReader
{
    public Text Label;
    public Server Server;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var bytesLength = stream.ReadInt(ref context);
        var bytes = stream.ReadBytesAsArray(ref context, bytesLength);
        var text = Encoding.ASCII.GetString(bytes);

        if (text.Length > 0)
        {
            if (Label.text.Length > 0)
                Label.text += "\n";

            Label.text += $"{connectionId} says: {text}";
        }

        // This means you're the server.
        if (Server != null) 
        {
            var writer = new DataStreamWriter(bytes.Length + 8, Allocator.Temp);

            writer.Write(Id);
            writer.Write(bytes.Length);
            writer.Write(bytes);

            Server.WriteToAllConnections(writer);
        }
    }
}
