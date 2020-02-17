using UnityEngine;

public class GunController : MonoBehaviour
{
    public Spawner Spawner;
    public ClientBehaviour Client;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Spawner.RequestSpawn(9999, Client, transform.position, transform.rotation);
        }
    }
}