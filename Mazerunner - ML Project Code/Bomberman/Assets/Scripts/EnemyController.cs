using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] target;
    [SerializeField] private float moveSpeed = 1f;

    Rigidbody myRigidBody;

    private bool isMoving = true;
    private int waypointDestination = 0;
    private bool movingForward = true;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            myRigidBody.MovePosition(Vector3.MoveTowards(transform.position, target[waypointDestination].position, Time.deltaTime * moveSpeed));
            if (Vector3.Distance(transform.position, target[waypointDestination].position) < 0.1f)
            {
                if (movingForward)
                {
                    if (waypointDestination >= target.Length - 1)
                    {
                        movingForward = false;
                        waypointDestination--;
                    }
                    // If enemy is moving forward AND has not reached the end of the waypoint array
                    else
                    {
                        waypointDestination++;
                    }
                }
                // Enemy is moving backward
                else
                {
                    if (waypointDestination <= 0)
                    {
                        movingForward = true;
                        waypointDestination++;
                    }
                    // If the enemy is moving backwards AND he has not reached the first waypoint in the array
                    else
                    {
                        waypointDestination--;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isMoving = false;
            Debug.Log("Enemy Controller : Enemy hit player");
        }
        if (collision.gameObject.tag == "Bomb")
        {
            isMoving = false;
            Debug.Log("Enemy Controller : Enemy hit bomb");
        }
    }
}
