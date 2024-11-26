using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowYRotation : MonoBehaviour
{
    [Header("minimap rotations")]
    private Transform playerTransform;
    public float playerOffset = 10f;

    private void Update()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + playerOffset, playerTransform.position.z);
            //transform.rotation = Quaternion.Euler(90f, playerReference.eulerAngles.y, 0f);
        }
    }

    public void SetPlayer(GameObject player)
    {
        playerTransform = player.transform;
    }
}
