using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

// This script controls the movement of ground type enemies along a path of waypoints.
public class EnemyBase : MonoBehaviour, IDamageable {
    private List<Transform> waypoints; // List of waypoints for the enemy to follow.
    private int currentWaypointIndex = 0; // Index of the current waypoint in the list.
    private NavMeshAgent agent; // Reference to the NavMeshAgent component.
    public float rotationSpeed = 5f; // Speed at which the enemy rotates toward the next waypoint.
    public float totalDistanceLeft; // Total distance left to reach the end of the path.
    [SerializeField] private int health = 1; // The health of the enemy.
    [SerializeField] private Transform centerpoint; // The centerpoint of the enemy.

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);

        // Get the sorted waypoints from the WaypointManager.
        if (WaypointManager.Instance != null) {
            waypoints = WaypointManager.Instance.GetWaypoints();
        }
        else {
            Debug.LogError("WaypointManager instance not found!");
        }

        if (waypoints != null && waypoints.Count > 0) {
            MoveToNextWaypoint();
        }
        else {
            Debug.LogError("No waypoints found!");
        }
    }

    // Checks if the enemy reached its current waypoint and rotates it toward the next target.
    void Update() {
        if (agent.remainingDistance < 0.5f && !agent.pathPending) {
            if (currentWaypointIndex < waypoints.Count - 1) {
                currentWaypointIndex++;
                MoveToNextWaypoint();
            }
            else {
                StopMovement();
            }
        }
        RotateTowards(agent.steeringTarget);
        UpdateTotalDistanceLeft(); // Update remaining distance each frame
    }

    // Sets the NavMeshAgent's destination to the current waypoint.
    void MoveToNextWaypoint() {
        if (currentWaypointIndex < waypoints.Count) {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    // Stops the NavMeshAgent.
    void StopMovement() {
        agent.isStopped = true;
    }

    // Manually rotates the enemy toward a target position while it is moving.
    void RotateTowards(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Reduces the enemy's health by the specified damage amount.
    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    // New helper method to update totalDistanceLeft based on current path
    void UpdateTotalDistanceLeft() {
        if (waypoints == null || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) {
            totalDistanceLeft = 0;
            return;
        }
        float distance = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        for (int i = currentWaypointIndex; i < waypoints.Count - 1; i++) {
            distance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
        totalDistanceLeft = distance;
    }

    public Vector3 CenterPoint() => centerpoint.position;
}