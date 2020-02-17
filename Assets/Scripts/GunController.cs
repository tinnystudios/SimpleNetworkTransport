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
            Spawner.RequestSpawn(1, Client, transform.position, transform.rotation);
        }
    }
}