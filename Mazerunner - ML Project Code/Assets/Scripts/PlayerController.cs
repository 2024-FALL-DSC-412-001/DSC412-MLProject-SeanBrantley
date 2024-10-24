using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 1f;

    Rigidbody myRigidBody;
    
    [SerializeField] GameObject bombPrefab;

    private GameManager myGameManager;
    
    private int bombSupply = 1;
    private int currentBombsPlaced = 0;

    private bool hasControl = true;

    [SerializeField] private float playerDestroyTime = 2f;

    private bool isPaused = false;
    private bool isDead = false;

    [SerializeField] private LayerMask theBombsLayers;

    private Animator myAnimator;
    private AudioSource myAudioSource;

    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip powerUpPickUpSound;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        if (myRigidBody == null)
        {
            Debug.LogError("Rigidbody component missing on player.");
        }
        myAnimator = GetComponent<Animator>();
        myGameManager = Object.FindFirstObjectByType<GameManager>();
        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Directly check the inputs
        //float x = Input.GetAxisRaw("Horizontal");
        //float z = Input.GetAxisRaw("Vertical");

        //Debug.Log($"PlayerController Input - Horizontal: {x}, Vertical: {z}");
        //bug.Log($"hasControl: {hasControl}, isPaused: {isPaused}");
        if (hasControl && !isPaused)
        {
            Movement();
            Rotation();
            UpdateAnimator();
            PlaceBomb();
        }
    }

    private void Rotation()
    {
        if (myRigidBody.velocity != Vector3.zero)
        {
            transform.forward = myRigidBody.velocity;
        }
    }

    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Apply the movement based on input, keeping y velocity unchanged
        Vector3 newVelocity = new Vector3(x * moveSpeed, myRigidBody.velocity.y, z * moveSpeed);
        myRigidBody.velocity = newVelocity;

        //Debug.Log($"MoveX: {x}, MoveZ: {z}, NewVelocity: {newVelocity}");

    }

    private void PlaceBomb()
    {

        if (Input.GetButtonDown("Fire1") && (currentBombsPlaced < bombSupply))
        {
            Vector3 center = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));

            // Create an overlap sphere where the new bomb will be placed to check if there is already a bomb there
            Collider[] hitColliders = Physics.OverlapSphere(center, 0.5f, theBombsLayers);
            if (hitColliders.Length > 0)
            {
                return;
            }
            
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.position = center;
            currentBombsPlaced++;
        }
    }

    public void TriggerBombPlacement()
    {
        PlaceBomb();
    }

    public void MoveAgent(float moveX, float moveZ)
    {
        //Debug.Log($"MoveAgent called with inputs: {moveX}, {moveZ}");
        Vector3 newVelocity = new Vector3(moveX * moveSpeed, 0f, moveZ * moveSpeed);
        myRigidBody.velocity = newVelocity;

        // Update player rotation based on movement
        if (myRigidBody.velocity != Vector3.zero)
        {
            transform.forward = myRigidBody.velocity;
        }

        UpdateAnimator();  // Make sure the animation updates when the agent moves
    }

    public int GetBombSupply()
    {
        return bombSupply;
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Play death sound
            myAudioSource.PlayOneShot(playerDeathSound, 1f);

            // After death player no longer can move or place bombs
            hasControl = false;

            // Removes all of players velocity since we now have no control of the player
            myRigidBody.velocity = Vector3.zero;

            myRigidBody.isKinematic = true;

            // Destroy player after set delay time
            Destroy(gameObject, playerDestroyTime);

            // Tell GameManager player has died
            myGameManager.PlayerDied();

            // Play death animation
            myAnimator.SetBool("isDefeat", true);
        }
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
        currentBombsPlaced--;
    }

    public float GetDestroyDelayTime()
    {
        return playerDestroyTime;
    }

    // Called when the player is spawned
    public void InitializePlayer(int bombs, float speed)
    {
        bombSupply = bombs;
        moveSpeed = speed;
    }

    public void SetPaused(bool state)
    {
        isPaused = state;
    }

    private void UpdateAnimator()
    {
        // If player has no velocity play the idle animation
        if (myRigidBody.velocity == Vector3.zero)
        {
            myAnimator.SetBool("isWalking", false);
        }
        else
        {
            myAnimator.SetBool("isWalking", true);
        }
    }

    public void PlayerVictory()
    {
        hasControl = false ;
        myAnimator.SetBool("isVictory", true);
    }

    public void PlayPowerUpPickUpSound()
    {
        // Play death sound
        myAudioSource.PlayOneShot(powerUpPickUpSound, 0.5f);
    }
}