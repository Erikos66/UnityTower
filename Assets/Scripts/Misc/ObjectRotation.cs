using UnityEngine;
using UnityEngine.AI;

public class RotateObject : MonoBehaviour {
    [SerializeField] private Vector3 rotationVector; // Vector representing the axis of rotation.
    [SerializeField] private float fallbackSpeed = 1f;         // Fallback speed if no parent NavMeshAgent is found.
    [SerializeField] private float rotationMultiplier = 100f;    // Multiplier to adjust the rotation speed.

    private NavMeshAgent parentAgent;

    // Gets the parent NavMeshAgent component; if not found, logs a warning.
    private void Start() {
        parentAgent = GetComponentInParent<NavMeshAgent>();
    }

    // Updates the wheel rotation using either the parent's speed or the fallback speed.
    void Update() {
        float speed = (parentAgent != null) ? parentAgent.speed : fallbackSpeed;
        float newRotationSpeed = speed + 1 * rotationMultiplier;
        transform.Rotate(rotationVector * newRotationSpeed * Time.deltaTime);
    }
}
