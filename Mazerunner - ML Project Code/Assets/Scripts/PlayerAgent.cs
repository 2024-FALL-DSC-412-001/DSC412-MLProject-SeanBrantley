using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAgent : Agent
{
    private PlayerController playerController;

    // Input Actions (replace with your actual input action names)
    private InputAction moveAction;
    private InputAction bombAction;

    public override void Initialize()
    {
        // Find PlayerController on the current GameObject first
        playerController = GetComponent<PlayerController>();

        // If PlayerController is not found, try finding it elsewhere in the scene
        if (playerController == null)
        {
            playerController = Object.FindFirstObjectByType<PlayerController>();
        }

        // If PlayerController still cannot be found, log an error
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is missing in PlayerAgent.");
        }

        // If you don't have a PlayerInput component, manually create InputAction
        moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        bombAction = new InputAction("Bomb", InputActionType.Button, "<Keyboard>/space");

        // Enable actions to start receiving input
        moveAction.Enable();
        bombAction.Enable();
    }

    void FixedUpdate()
    {
        // Request decision every frame (for more frequent decisions)
        RequestDecision();

        // Debug input values
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        //Debug.Log($"Update Move Input: {moveInput.x}, {moveInput.y}");
        //Debug.Log($"Update Bomb Action: {bombAction.triggered}");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        int placeBomb = actions.DiscreteActions[0];

        // Move and bomb actions passed to the controller
        playerController.MoveAgent(moveX, moveZ);

        if (placeBomb == 1)
        {
            playerController.TriggerBombPlacement();
            AddReward(2.0f);

            // Start bomb blast dodge check
            StartCoroutine(CheckForBombDodge());
        }

        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0)
        {
            AddReward(1.0f);  // Reward for moving
        }
        else
        {
            AddReward(-2.0f);  // Penalty for standing still
        }
    }

    private IEnumerator CheckForBombDodge()
    {
        // Define how long before the bomb explodes
        float bombTimer = 2.0f;  // Example: 3 seconds until explosion

        // Define the bomb's position or blast radius
        Vector3 bombPosition = playerController.transform.position;

        // Wait until the bomb is about to explode
        yield return new WaitForSeconds(bombTimer);

        // Check if the agent is inside the danger zone (blast radius)
        float distanceToBomb = Vector3.Distance(playerController.transform.position, bombPosition);
        float blastRadius = 4.0f;  // Example: blast radius of 5 units

        if (distanceToBomb > blastRadius)
        {
            // Agent dodged the bomb, give a reward
            AddReward(5.0f);
            Debug.Log("Agent dodged the bomb!");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations (player's position, bomb supply, etc.)
        sensor.AddObservation(transform.position);
        // Agent's velocity
        sensor.AddObservation(GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(playerController.GetBombSupply());
    }

    public void AddVictoryReward()
    {
        AddReward(100.0f);  // Add the victory reward
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;

        // Read movement input from the Input System
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        //Debug.Log($"Heuristic Input - Horizontal: {moveInput.x}, Vertical: {moveInput.y}");
        continuousActions[0] = moveInput.x;
        continuousActions[1] = moveInput.y;

        // Check if the bomb action was triggered
        discreteActions[0] = bombAction.triggered ? 1 : 0;

        //Debug.Log($"Heuristic MoveX: {continuousActions[0]}, MoveZ: {continuousActions[1]}, BombAction: {discreteActions[0]}");
    }
}