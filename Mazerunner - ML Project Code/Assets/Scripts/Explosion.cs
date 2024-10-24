using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SphereCollider bomb;
    Rigidbody myRigidBody;
    private Vector3 explosionDirection = Vector3.zero;
    private float explosionSpeed = 200f;
    private float explosionRange = 2f;
    private Vector3 startPosition;
    PlayerAgent agent;

    void Start()
    {
        bomb = GameObject.FindGameObjectWithTag("Player").GetComponent<SphereCollider>();
        myRigidBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, startPosition) >= explosionRange)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        myRigidBody.velocity = explosionSpeed * Time.deltaTime * explosionDirection;
    }

    public void setExplosion(Vector3 diection, float speed, float range)
    {
        explosionDirection = diection;
        explosionSpeed = speed;
        explosionRange = range;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Block":
                {
                    Destroy(gameObject);
                    break;
                }
            case "Bomb":
                {
                    other.gameObject.GetComponent<Bomb>().Explode();
                    Destroy(gameObject);
                    break;
                }
            case "Player":
                {
                    other.gameObject.GetComponent<PlayerController>().Die();
                    break;
                }
            case "Enemy":
                {
                    other.gameObject.GetComponent<EnemyController>().Die();
                    break;
                }
            case "Destructible":
                {
                    agent.AddReward(2.0f);
                    Object.FindFirstObjectByType<PowerUpSpawner>().BlockDestroyed(other.transform.position);

                    // Plau destroy block animation
                    other.gameObject.GetComponent<Animator>().SetTrigger("isDestroyed");

                    Destroy(other.gameObject, 0.75f);
                    Destroy(gameObject);
                    break;
                }
            case "PowerUp":
                {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                    break;
                }
        }
    }
}
