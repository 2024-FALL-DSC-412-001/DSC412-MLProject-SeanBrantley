using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float xMoveSpeed;
    [SerializeField] float yMoveSpeed;
    [SerializeField] float zMoveSpeed;
    
    Rigidbody myRigidBody;
    
    [SerializeField] GameObject bombPrefab;

    private GameManager myGameManager;
    
    [SerializeField] private int bombSupply = 2;
    private int currentBombsPlaced = 0;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myGameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        Movement();
        PlaceBomb();
    }

    void Movement()
    {
        Vector3 newVelocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            newVelocity = new Vector3(0f, 0f, zMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            newVelocity = new Vector3(0f, 0f, -zMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            newVelocity = new Vector3(-xMoveSpeed, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            newVelocity = new Vector3(xMoveSpeed, 0f, 0f);
        } 
        myRigidBody.velocity = newVelocity;
    }

    private void PlaceBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (currentBombsPlaced < bombSupply))
        {
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
            currentBombsPlaced++;
        }
    }

    private void Die()
    {
        // Tell GameManager player has died
        myGameManager.PlayerDied();
        
        // Player death animation
        // Remove player from scene
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // Player hit enemy and died.
            Die();
        }
    }

    public void BombExploded()
    {
       Debug.Log("Hello" + currentBombsPlaced);
       currentBombsPlaced--;
    }
}