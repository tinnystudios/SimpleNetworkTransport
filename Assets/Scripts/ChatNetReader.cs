using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class ChatNetReader : NetReader
{
    public Text Label;
    public bool AddText = true;

    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var text = stream.ReadString(ref context);

        if (text.LengthInBytes > 0)
        {
            if (AddText)
            {
                if (Label.text.Length > 0)
                    Label.text += "\n";

                Label.text += $"{connectionId} says: {text}";
            }
            else
            {
                Label.text = text.ToString();
            }
        }
    }
}
