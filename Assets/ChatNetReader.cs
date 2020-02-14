using Unity.Networking.Transport;
using UnityEngine.UI;

public class ChatNetReader : NetReader
{
    public Text Label;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var text = stream.ReadString(ref context);

        if (text.LengthInBytes > 0)
        {
            if (Label.text.Length > 0)
                Label.text += "\n";

            Label.text += $"{connectionId} says: {text}";
        }
    }
}
