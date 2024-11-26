using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int lives = 3;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerParentTransform;

    void Start()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(0,0,0), Quaternion.identity, playerParentTransform);
    }

    public void PlayerDied()
    {
        if (lives > 1)
        {
            Debug.Log("Player died");
            lives--;
        }
        else
        {
            Debug.Log("GameOver");
        }
    }
}
