using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for towers, handling targeting, rotation, and attacking behaviors.
/// </summary>
public class Tower : MonoBehaviour {
    public enum TargetMode { Closest, Random, HighestThreat } // Enum for different targeting modes.

    [SerializeField] protected float attackCooldown = 1f; // Attack cooldown duration in seconds.
    protected float lastTimeAttacked; // Timestamp for the last attack.
    private bool canRotate = true; // Flag indicating if the tower head can rotate.

    [Header("Targeting Settings")]
    [SerializeField] protected TargetMode targetMode = TargetMode.Random; // Selected targeting mode.
    [SerializeField] protected float attackRange = 3f; // Maximum distance at which the tower can target enemies.
    [SerializeField] protected Transform towerHead; // The transform representing the tower head that rotates towards the enemy.
    [SerializeField] protected float rotationSpeed = 5f; // Speed at which the tower head rotates.
    protected EnemyBase currentEnemy; // Reference to the current target enemy.

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
    } // Ignore this comment

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
            case TargetMode.HighestThreat:
                FindHighestThreatEnemy(hitEnemies);
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
        foreach (var enemyCollider in hitEnemies) {
            EnemyBase enemyComponent = enemyCollider.GetComponent<EnemyBase>();
            if (enemyComponent == null) continue;
            float distanceSqr = (transform.position - enemyComponent.CenterPoint()).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr) {
                closestDistanceSqr = distanceSqr;
                currentEnemy = enemyComponent;
            }
        }
    }

    /// <summary>
    /// Finds and assigns a random enemy within the attack range if a valid target is not already assigned.
    /// </summary>
    /// <param name="hitEnemies">Array of enemy colliders within range.</param>
    protected void FindRandomEnemy(Collider[] hitEnemies) {
        if (hitEnemies.Length == 0 ||
            (currentEnemy != null && (transform.position - currentEnemy.CenterPoint()).sqrMagnitude <= attackRange * attackRange)) {
            return;
        }
        List<Collider> validEnemies = new List<Collider>();
        foreach (var enemyCollider in hitEnemies) {
            if (enemyCollider.GetComponent<EnemyBase>() != null)
                validEnemies.Add(enemyCollider);
        }
        if (validEnemies.Count == 0) return;
        int randomIndex = Random.Range(0, validEnemies.Count);
        currentEnemy = validEnemies[randomIndex].GetComponent<EnemyBase>();
    }

    /// <summary>
    /// Finds and assigns the enemy with the highest threat level within the attack range.
    /// </summary>
    /// <param name="hitEnemies"></param>
    protected void FindHighestThreatEnemy(Collider[] hitEnemies) {
        currentEnemy = null;
        float minDistanceLeft = float.MaxValue;
        foreach (var enemyCollider in hitEnemies) {
            EnemyBase enemyMovement = enemyCollider.GetComponent<EnemyBase>();
            if (enemyMovement != null) {
                if (enemyMovement.totalDistanceLeft < minDistanceLeft) {
                    minDistanceLeft = enemyMovement.totalDistanceLeft;
                    currentEnemy = enemyMovement;
                }
            }
        }
    }

    /// <summary>
    /// Rotates the tower head smoothly towards the current enemy.
    /// </summary>
    protected virtual void RotateTowardsEnemy() {
        if (!canRotate || currentEnemy == null) return;
        Vector3 directionToEnemy = currentEnemy.CenterPoint() - towerHead.position;
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
        return (currentEnemy.CenterPoint() - startPoint.position).normalized;
    }

    /// <summary>
    /// Enables or disables the rotation of the tower head.
    /// </summary>
    /// <param name="enable">If true, tower head rotation is enabled; otherwise, it is disabled.</param>
    public void EnableRotation(bool enable) {
        canRotate = enable;
    }
}