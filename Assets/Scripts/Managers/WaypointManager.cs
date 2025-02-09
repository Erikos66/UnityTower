using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaypointManager : MonoBehaviour {
    public static WaypointManager Instance { get; private set; }
    private List<Transform> waypoints = new List<Transform>();

    // Called when the object is created; sets up the singleton and finds/sorts waypoints.
    void Awake() {
        if (Instance == null) {
            Instance = this;
            FindAndSortWaypoints();
            HideWaypoints();
        }
        else {
            Destroy(gameObject);
        }
    }

    // Finds the child waypoint objects, sorts them alphabetically, and stores them in the list.
    void FindAndSortWaypoints() {
        waypoints = GetComponentsInChildren<Transform>(false)
            .Where(t => t != transform)
            .OrderBy(t => t.name)
            .ToList();
    }

    // Hides the visual representations of the waypoints by disabling any Renderer components.
    void HideWaypoints() {
        foreach (Transform wp in waypoints) {
            Renderer rend = wp.GetComponent<Renderer>();
            if (rend != null) {
                rend.enabled = false;
            }
        }
    }

    // Public method for retrieving the sorted list of waypoints.
    public List<Transform> GetWaypoints() {
        return waypoints;
    }
}