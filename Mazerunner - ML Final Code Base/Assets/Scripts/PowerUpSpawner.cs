using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    // An array of all the power up type prefabs
    [SerializeField] GameObject[] powerUpPrefabs;


    // This method is called everytime a destructible block is destroyed
    // Will check to see if a random power up should be spawned
    public void BlockDestroyed(Vector3 pos)
    {
        if (Random.value < 0.2)
        {
            Instantiate(powerUpPrefabs[Random.Range (0, powerUpPrefabs.Length)], pos, Quaternion.identity);
        }
    }
}
