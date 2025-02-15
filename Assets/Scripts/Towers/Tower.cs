using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for towers, handling targeting, rotation, and attacking behaviors.
/// </summary>
public class Tower : MonoBehaviour {
    public enum TargetMode { Closest, Random } // Enumeration for the different targeting modes.

    [SerializeField] protected float attackCooldown = 1f; // Attack cooldown duration in seconds.
    protected float lastTimeAttacked; // Timestamp for the last attack.
    private bool canRotate = true; // Flag indicating if the tower head can rotate.

    [Header("Targeting Settings")]
    [SerializeField] protected TargetMode targetMode = TargetMode.Random; // Selected targeting mode.
    [SerializeField] protected float attackRange = 3f; // Maximum distance at which the tower can target enemies.
    [SerializeField] protected Transform towerHead; // The transform representing the tower head that rotates towards the enemy.
    [SerializeField] protected float rotationSpeed = 5f; // Speed at which the tower head rotates.
    protected Transform currentEnemy; // Reference to the current target enemy.

    /// <summary>
    /// Unity Update method to process targeting, rotation, and attacking.
    /// </summary>
    protected virtual void Update() {
        FindTarget();
        RotateTowardsEnemy();
        if (currentEnemy != null && Time.time - lastTimeAttacked >= attackCooldown) {
            lastTimeAttacked = Time.time;
            Attack();
        }
    }

    /// <summary>
    /// Unity Awake method.
    /// </summary>
    protected virtual void Awake() {
    }

    /// <summary>
    /// Finds and assigns the target enemy based on the selected targeting mode.
    /// </summary>
    protected virtual void FindTarget() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        switch (targetMode) {
            case TargetMode.Closest:
                FindClosestEnemy(hitEnemies);
                break;
            case TargetMode.Random:
                FindRandomEnemy(hitEnemies);
                break;
            default:
                Debug.LogWarning("Invalid target mode on tower " + name);
                break;
        }
    }

    /// <summary>
    /// Initiates an attack on the current target enemy.
    /// </summary>
    protected virtual void Attack() {
        if (currentEnemy == null) return;
    }

    /// <summary>
    /// Finds and assigns the closest enemy within the attack range.
    /// </summary>
    /// <param name="hitEnemies">Array of enemy colliders within range.</param>
    protected void FindClosestEnemy(Collider[] hitEnemies) {
        currentEnemy = null;
        float closestDistanceSqr = attackRange * attackRange;
        foreach (var enemy in hitEnemies) {
            float distanceSqr = (transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr) {
                closestDistanceSqr = distanceSqr;
                currentEnemy = enemy.transform;
            }
        }
    }

    /// <summary>
    /// Finds and assigns a random enemy within the attack range if a valid target is not already assigned.
    /// </summary>
    /// <param name="hitEnemies">Array of enemy colliders within range.</param>
    protected void FindRandomEnemy(Collider[] hitEnemies) {
        if (hitEnemies.Length == 0 ||
            (currentEnemy != null && (transform.position - currentEnemy.position).sqrMagnitude <= attackRange * attackRange)) {
            return;
        }
        int randomIndex = Random.Range(0, hitEnemies.Length);
        currentEnemy = hitEnemies[randomIndex].transform;
    }

    /// <summary>
    /// Rotates the tower head smoothly towards the current enemy.
    /// </summary>
    protected virtual void RotateTowardsEnemy() {
        if (!canRotate || currentEnemy == null) return;
        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        towerHead.rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// Draws a visual gizmo for the attack range in the Unity editor.
    /// </summary>
    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    /// <summary>
    /// Calculates the normalized direction vector from a given start point to the current enemy.
    /// </summary>
    /// <param name="startPoint">The starting point to measure from.</param>
    /// <returns>The normalized direction vector or Vector3.zero if no current enemy exists.</returns>
    protected Vector3 DirectionToEnemy(Transform startPoint) {
        if (currentEnemy == null) return Vector3.zero;
        return (currentEnemy.position - startPoint.position).normalized;
    }

    /// <summary>
    /// Enables or disables the rotation of the tower head.
    /// </summary>
    /// <param name="enable">If true, tower head rotation is enabled; otherwise, it is disabled.</param>
    public void EnableRotation(bool enable) {
        canRotate = enable;
    }
}