using Unity.Networking.Transport;

namespace SimpleTransport
{
    public class ChatRPC : RPC<string>
    {
        public override int Capacity => 8;
        public override int Id => 5;

        public override void Write(DataStreamWriter writer, string data)
        {
            writer.WriteString(data);
        }

        public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
        {
            Data = reader.ReadString(ref context).ToString();
        }
    }
}