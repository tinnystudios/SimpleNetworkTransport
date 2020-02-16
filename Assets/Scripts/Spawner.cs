using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            // Make Cube
            // Make a ghost
        }
    }

    // Ghost

}