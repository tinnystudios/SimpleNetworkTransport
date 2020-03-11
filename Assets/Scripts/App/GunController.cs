using SimpleTransport;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public SpawnSystem Spawner;
    public NetworkClient Client;
    public Transform Target;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            Spawner = Spawner ?? FindObjectOfType<SpawnSystem>();
            Client = Client ?? GetComponentInParent<NetworkClient>();

            var pos = Target.position + Target.forward * 1.0F;
            Spawner.SpawnRequest(1, Client, pos, Target.rotation);
        }
    }
}