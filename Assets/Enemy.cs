using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    private NavMeshAgent _agent;
    private int _currentWaypointIndex;

    [SerializeField] private Transform[] waypoint;
    private void Awake() {
        // Gets the components from the parent object
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        Debug.Log("Here we go...");
    }

    /*
     * This method returns the next waypoint in the array and increments the current waypoint index.
     * If the index is greater than the length of the array it resets the index to 0.
     * Which in this case, is the start point.
     * Considering arrays cant even be modified in runtime, this is dumb as hell.
     * Might be worth using a list, but I feel like this whole file is a throwaway.
     */
    private Vector3 GetNextWaypoint() {
        var targetPoint = waypoint[_currentWaypointIndex].position;
        _currentWaypointIndex++;
        return _currentWaypointIndex >= waypoint.Length ? transform.position : targetPoint;
    }

    private void FaceTarget(Vector3 newTarget) {
        return;
    }
    // I dont think using update for this is very effective it really doesn't need to check if its reached the waypoint every frame.
    // TODO: #2 Use collision events to detect when the enemy reaches the waypoint instead.
    private void Update() {
        if (!(_agent.remainingDistance < 0.2f)) return;
        var nextWaypoint = GetNextWaypoint();
        _agent.SetDestination(nextWaypoint);
    }
}
