using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerAgent : Agent
{
    private GameManager gameManager;
    private PlayerController playerController;

    private GameObject[] waypoints;
    private HashSet<Vector3> exploredPositions = new HashSet<Vector3>();
    private int waypointsCollected = 0;
    private int decisionFrequency = 5; // Make decisions every 5 frames
    private int frameCount = 0;
    private float timeLimit = 180f; // 3-minute time limit
    private float timeElapsed;

    private float radarPingTimer = 0f; // Time since the last radar ping
    private float radarPingInterval = 1f; // Radar ping every second

    private int episodeCount = 0;

    // Input Actions (replace with your actual input action names)
    private InputAction moveAction;

    RayPerceptionSensorComponent3D rayPerceptionSensor;

    public override void Initialize()
    {
        playerController = GetComponent<PlayerController>();
        gameManager = GameObject.FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager reference is missing in PlayerAgent.");
        }

        if (playerController == null)
        {
            playerController = Object.FindFirstObjectByType<PlayerController>();
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is missing in PlayerAgent.");
        }

        moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.Enable();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        if (waypoints.Length == 0) Debug.LogError("Not enough waypoints found in the scene.");
    }

    void Start()
    {
        rayPerceptionSensor = GetComponent<RayPerceptionSensorComponent3D>();
    }

    void FixedUpdate()
    {
        frameCount++;
        if (frameCount >= decisionFrequency)
        {
            RequestDecision();
            frameCount = 0; // Reset counter after decision request
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        playerController.MoveAgent(moveX, moveZ);

        Vector3 currentPos = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );

        // Increment elapsed time and check if the episode should end
        timeElapsed += Time.deltaTime;
        radarPingTimer += Time.deltaTime;

        if (timeElapsed >= timeLimit)
        {
            //AddReward(-5.0f); // Small penalty for not completing in time
            EndEpisode();
            Debug.Log("Episode ended due to time limit.");
            return;
        }

        // Radar punishment structure
        if (radarPingTimer >= radarPingInterval)
        {
            
            radarPingTimer = 0f; // Reset the radar ping timer

            float closestWaypointDistance = float.MaxValue;

            foreach (var waypoint in waypoints)
            {
                if (waypoint.GetComponent<Waypoint>().collected) continue;

                float distance = Vector3.Distance(transform.position, waypoint.transform.position);
                if (distance < closestWaypointDistance)
                {
                    closestWaypointDistance = distance;
                }
            }

            // Round the distance to the nearest integer
            int roundedDistance = Mathf.RoundToInt(closestWaypointDistance);
            Debug.Log(roundedDistance);
            if (roundedDistance > 4)
            {
                // Penalize the agent based on the rounded distance
                float penalty = -1.0f * roundedDistance;
                AddReward(penalty);

                Debug.Log($"Radar ping: Closest waypoint distance (rounded): {roundedDistance}. Penalty applied: {penalty}");
            }
        }

        //Exploration Reward
        if (!exploredPositions.Contains(currentPos))
        {
            exploredPositions.Add(currentPos);
            AddReward(30.0f);
            Debug.Log($"Explored new area: {currentPos}");

        }

        // Small reward for each movement to encourage exploration
        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0)
        {
            AddReward(0.1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) return;
        //Debug.Log($"Collision detected with {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Destructible"))
        {
            AddReward(-0.2f);  // Penalty for hitting obstacles
            Debug.Log("-0.2 for hitting obstacle");
        }
        else if (collision.gameObject.CompareTag("Waypoint"))
        {
            Waypoint waypoint = collision.gameObject.GetComponent<Waypoint>();
            if (waypoint != null && !waypoint.collected)
            {
                waypoint.collected = true;
                waypointsCollected++;
                AddReward(1000f);
                Debug.Log("Collected waypoint.");

                collision.gameObject.SetActive(false);

                if (waypointsCollected >= 1)
                {
                    gameManager.WinGame();
                    EndEpisode();
                }
            }
        }
    }

    public void AddCompletionBonusReward(float rewardAmount)
    {
        AddReward(rewardAmount);
        Debug.Log("Bonus reward applied for completing faster than previous best time!");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var waypoint in waypoints)
        {
            if (waypoint.GetComponent<Waypoint>().collected)
            {
                // Add a zero vector if the waypoint has been collected
                sensor.AddObservation(Vector3.zero);
            }
            else
            {
                // Add the relative position of the waypoint if it hasn't been collected
                sensor.AddObservation(waypoint.transform.position - transform.position);
            }
        }

        foreach (var block in GameObject.FindGameObjectsWithTag("Block"))
        {
            sensor.AddObservation(block.transform.position - transform.position);
        }

        // Additional observations, like player position or velocity, can be added here
        sensor.AddObservation(transform.position);
        sensor.AddObservation(GetComponent<Rigidbody>().velocity);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = moveAction.ReadValue<Vector2>().x;  // Movement on x-axis
        continuousActions[1] = moveAction.ReadValue<Vector2>().y;  // Movement on z-axis
    }

    public override void OnEpisodeBegin()
    {
        waypointsCollected = 0;
        exploredPositions.Clear();
        timeElapsed = 0f; // Reset the timer
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;

        // Set the agent's position to a fixed starting point (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;  // Reset rotation to default
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        episodeCount = episodeCount + 1;

        // Reset all waypoints
        foreach (var waypoint in waypoints)
        {
            waypoint.SetActive(true);
            var waypointComponent = waypoint.GetComponent<Waypoint>();
            if (waypointComponent != null)
            {
                waypointComponent.collected = false;
            }
        }

        Debug.Log("New episode started. Time limit: " + timeLimit + " seconds.");
    }
}
