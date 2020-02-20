using Unity.Networking.Transport;

public class DespawnReader : NetReader
{
    public Spawner Spawner;
    public override void Read(int connectionId, DataStreamReader stream, ref DataStreamReader.Context context)
    {
        var instanceId = stream.ReadInt(ref context);
        Spawner.DespawnInClient(instanceId, GetComponentInParent<Client>());
    }
}