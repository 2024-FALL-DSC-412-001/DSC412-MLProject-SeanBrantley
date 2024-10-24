using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] target;
    [SerializeField] private float moveSpeed = 1f;

    Rigidbody myRigidBody;
    AudioSource myAudioSource;

    private bool isMoving = true;
    private int waypointDestination = 0;
    private bool movingForward = true;

    [SerializeField] private float minDelayTime = .25f;
    [SerializeField] private float maxDelayTime = 2f;

    //[SerializeField] private int scoreValue = 50;

    private bool isDead = false;

    private Animator myAnimator;

    void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();

        if (target.Length == 0)
        {
            isMoving = false;
            Debug.LogWarning("Enemy " + gameObject.name + " has no waypoints");
        }
    }

    private void Update()
    {
        UpdateAnimator();
    }

    void FixedUpdate()
    {

        if (isDead) { return; }

        if (isMoving)
        {
            myRigidBody.MovePosition(Vector3.MoveTowards(transform.position, target[waypointDestination].position, Time.deltaTime * moveSpeed));
            transform.LookAt(target[waypointDestination].position);
            if (Vector3.Distance(transform.position, target[waypointDestination].position) < 0.1f)
            {
                isMoving = false;
                if (movingForward)
                {
                    if (waypointDestination >= target.Length - 1)
                    {
                        movingForward = false;
                        Invoke("DecreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
                    }
                    // If enemy is moving forward AND has not reached the end of the waypoint array
                    else
                    {
                        Invoke("IncreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
                    }
                }
                // Enemy is moving backward
                else
                {
                    if (waypointDestination <= 0)
                    {
                        movingForward = true;
                        Invoke("IncreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
                    }
                    // If the enemy is moving backwards AND he has not reached the first waypoint in the array
                    else
                    {
                        Invoke("DecreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //isMoving = false;
        }
        if (collision.gameObject.tag == "Bomb")
        {
            isMoving = false;
            if (movingForward)
            {
                Invoke("DecreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
            }
            else
            {
                Invoke("IncreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
            }
            movingForward = !movingForward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bomb")
        {
            isMoving = false;
            if (movingForward)
            {
                Invoke("DecreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
            }
            else
            {
                Invoke("IncreaseWaypointDestination", Random.Range(minDelayTime, maxDelayTime));
            }
            movingForward = !movingForward;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
            isMoving = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Bomb")
        {
            isMoving = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
            isMoving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bomb")
        {
            isMoving = true;
        }
    }


    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            GameManager myGameManager = Object.FindFirstObjectByType<GameManager>();
            //myGameManager.UpdateScore(scoreValue);
            myGameManager.EnemyHasDied();
            myAudioSource.Play();
            Destroy(gameObject, 3f);
            GetComponent<Collider>().enabled = false;
            myAnimator.SetBool("isDead", true);
        }
    }

    private void IncreaseWaypointDestination()
    {
        if (waypointDestination + 1 <= target.Length)
        {
            waypointDestination++;
        }
        isMoving = true;
    }

    private void DecreaseWaypointDestination()
    {
        if (waypointDestination - 1 >= 0)
        {
            waypointDestination--;
        }
        isMoving = true;
    }

    private void UpdateAnimator()
    {
        myAnimator.SetBool("isWalking", isMoving);
    }
}
