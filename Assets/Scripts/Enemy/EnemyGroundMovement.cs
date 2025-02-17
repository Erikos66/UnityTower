using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

// This script controls the movement of ground type enemies along a path of waypoints.
public class EnemyGroundMovement : MonoBehaviour, IDamageable {
    private List<Transform> waypoints; // List of waypoints for the enemy to follow.
    private int currentWaypointIndex = 0; // Index of the current waypoint in the list.
    private NavMeshAgent agent; // Reference to the NavMeshAgent component.
    public float rotationSpeed = 5f; // Speed at which the enemy rotates toward the next waypoint.
    [SerializeField] private int health = 1; // The health of the enemy.

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

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}