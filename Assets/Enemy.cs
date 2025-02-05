using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform[] waypoint;
    private int _currentWaypointIndex;

   
    private void Awake()
    {
        // Gets the components from the parent object
        _agent = GetComponent<NavMeshAgent>();
    }
    
    /*
     * This method returns the next waypoint in the array and increments the current waypoint index.
     * If the index is greater than the length of the array it resets the index to 0.
     * Which in this case, is the start point.
     * Considering arrays cant even be modified in runtime, this is dumb as hell.
     * Might be worth using a list, but I feel like this whole file is a throwaway.
     */
    private Vector3 GetNextWaypoint()
    {
        var targetPoint =  waypoint[_currentWaypointIndex].position;
        _currentWaypointIndex++;
        if (_currentWaypointIndex >= waypoint.Length)
        {
            _currentWaypointIndex = 0;
        }
        return targetPoint;
    } 
    
    // I dont think using update for this is very effective it really doesn't need to check if its reached the waypoint every frame.
    // TODO: Use collision events to detect when the enemy reaches the waypoint instead.
    private void Update()
    {
        if (_agent.remainingDistance < 0.1f)
        {
            _agent.SetDestination(GetNextWaypoint());
        }
    }
}
