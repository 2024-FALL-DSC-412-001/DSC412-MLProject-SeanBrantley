using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 0;

    Rigidbody myRigidBody;
    private bool hasControl = true;
    private bool isPaused = false;

    private Animator myAnimator;
    private AudioSource myAudioSource;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hasControl && !isPaused)
        {
            Movement();
            Rotation();
            UpdateAnimator();
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

        Vector3 newVelocity = new Vector3(x * moveSpeed, 0f, z * moveSpeed);
        myRigidBody.velocity = newVelocity;
    }

    public void MoveAgent(float moveX, float moveZ)
    {
        if (myRigidBody == null)
        {
            Debug.LogError("myRigidBody is null in MoveAgent");
        }

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

    // Called when the player is spawned
    public void InitializePlayer(float speed)
    {
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
}