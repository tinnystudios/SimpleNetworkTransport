using UnityEngine;

public class GunController : MonoBehaviour
{
    public Spawner Spawner;
    public ClientBehaviour Client;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Spawner = Spawner ?? FindObjectOfType<Spawner>();
            Client = Client ?? GetComponentInParent<ClientBehaviour>();

            var pos = transform.position + transform.forward * 1.0F;
            Spawner.RequestSpawn(1, Client, pos, transform.rotation);
        }
    }
}