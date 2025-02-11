using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public enum TargetMode { Closest, Random } // Targeting modes
    [SerializeField] protected float attackCooldown = 1f; // Attack cooldown of the tower
    protected float lastTimeAttacked; // Timer to keep track of the attack cooldown

    [Header("Targeting Settings")]
    [SerializeField] protected TargetMode targetMode = TargetMode.Random; // Default target mode
    [SerializeField] protected float attackRange = 3f; // Attack range of the tower
    [SerializeField] protected Transform towerHead; // Tower head transform to rotate towards enemy
    [SerializeField] protected float rotationSpeed = 5f; // Rotation speed of the tower head

    protected Transform currentEnemy;

    protected virtual void Update() {
        // Find the target enemy
        FindTarget();

        // Rotate the tower head towards the target enemy
        RotateTowardsEnemy();

        // Check if the attack cooldown has passed and the tower has a target enemy
        // If so, update the last time attacked and call the attack method
        if (currentEnemy != null && Time.time - lastTimeAttacked >= attackCooldown) {
            lastTimeAttacked = Time.time;
            Attack();
        }
    }

    /// <summary>
    /// Finds and assigns the target enemy based on the target mode
    /// </summary>
    protected virtual void FindTarget() {
        // Find all enemies within the attack range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        // Assign the target enemy based on the target mode
        switch (targetMode) {
            case TargetMode.Closest:
                // Find the closest enemy
                FindClosestEnemy(hitEnemies);
                break;
            case TargetMode.Random:
                // Find a random enemy
                FindRandomEnemy(hitEnemies);
                break;
            default:
                // If the target mode is invalid, do nothing
                break;
        }
    }

    protected virtual void Attack() {
        if (currentEnemy == null) return;
    }

    /// <summary>
    /// Finds and assigns the closest enemy within the attack range to the current enemy.
    /// </summary>
    /// <param name="hitEnemies">Array of colliders representing the enemies in range.</param>
    protected void FindClosestEnemy(Collider[] hitEnemies) {
        // Initialize currentEnemy to null
        currentEnemy = null;
        // Calculate attack range squared for distance comparison
        float closestDistanceSqr = attackRange * attackRange;

        // Iterate through all the enemies in range
        foreach (var enemy in hitEnemies) {
            // Calculate squared distance from the tower to the enemy
            float distanceSqr = (transform.position - enemy.transform.position).sqrMagnitude;
            // Check if this enemy is closer than the previously found ones
            if (distanceSqr < closestDistanceSqr) {
                closestDistanceSqr = distanceSqr;
                // Assign this enemy as the current closest enemy
                currentEnemy = enemy.transform;
            }
        }
    }

    /// <summary>
    /// Finds and assigns a random enemy in the attack range to the current enemy if it's not the same as the current enemy.
    /// </summary>
    /// <param name="hitEnemies">Array of colliders representing the enemies in range.</param>
    protected void FindRandomEnemy(Collider[] hitEnemies) {
        if (hitEnemies.Length == 0) {
            currentEnemy = null;
            return;
        }

        // Select a random enemy from the array
        int randomIndex = Random.Range(0, hitEnemies.Length);
        // Assign the selected enemy to the current enemy
        currentEnemy = hitEnemies[randomIndex].transform;
    }

    /// <summary>
    /// Rotates the tower head towards the current enemy.
    /// </summary>
    protected virtual void RotateTowardsEnemy() {
        if (currentEnemy == null) return;

        // Calculate the direction from the tower head to the current enemy
        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;
        // Calculate the look rotation from the direction
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        // Smoothly rotate the tower head towards the look rotation
        towerHead.rotation = Quaternion.Lerp(
            towerHead.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    // Draw attack range gizmo in the editor for visual representation
    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    /// <summary>
    /// Calculates the direction from a given start point to the current enemy, or zero if there is no current enemy.
    /// </summary>
    /// <param name="startPoint">The point from which to calculate the direction.</param>
    /// <returns>The direction from the start point to the current enemy, or zero if there is no current enemy.</returns>
    protected Vector3 DirectionToEnemy(Transform startPoint) {
        if (currentEnemy == null) return Vector3.zero;
        return (currentEnemy.position - startPoint.position).normalized;
    }
}