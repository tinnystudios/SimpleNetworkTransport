using SimpleTransport;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public SpawnSystem Spawner;
    public NetworkClient Client;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Spawner = Spawner ?? FindObjectOfType<SpawnSystem>();
            Client = Client ?? GetComponentInParent<NetworkClient>();

            var pos = transform.position + transform.forward * 1.0F;
            Spawner.SpawnRequest(1, Client, pos, transform.rotation);
        }
    }
}