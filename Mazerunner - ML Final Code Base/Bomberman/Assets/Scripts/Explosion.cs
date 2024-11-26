using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Rigidbody myRigidBody;
    private Vector3 explosionDirection = Vector3.zero;
    private float explosionSpeed = 200f;
    private float explosionRange = 2f;
    private Vector3 startPosition;

    void Start()
    {
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
        Debug.Log("Explosion has hit " + other.gameObject.name + " with the tag of " + other.gameObject.tag);
        
        if (other.gameObject.tag == "Block")
        {
            Destroy (gameObject);
        }
        
        if (other.gameObject.tag == "Bomb")
        {
           other.gameObject.GetComponent<Bomb>().Explode();
           Destroy(gameObject);
        }
    }
}
