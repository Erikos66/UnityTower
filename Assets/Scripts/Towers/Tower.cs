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
        FindTarget();
        RotateTowardsEnemy();
        if (currentEnemy != null && Time.time - lastTimeAttacked >= attackCooldown) {
            lastTimeAttacked = Time.time;
            Attack();
        }
    }

    // Find the target based on the target mode and assign it to the current enemy
    protected virtual void FindTarget() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (targetMode == TargetMode.Closest) {
            FindClosestEnemy(hitEnemies);
        }
        else if (targetMode == TargetMode.Random) {
            FindRandomEnemy(hitEnemies);
        }
    }

    protected virtual void Attack() {
        if (currentEnemy == null) return;
    }

    // Find the closest enemy in the attack range and assign it to the current enemy
    protected void FindClosestEnemy(Collider[] hitEnemies) {
        float closestDistance = attackRange;
        Transform closestEnemy = null;

        foreach (var enemy in hitEnemies) {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        currentEnemy = closestEnemy; // Assign closest enemy
    }

    // Find a random enemy in the attack range and assign it to the current enemy if it's not the same as the current enemy
    protected void FindRandomEnemy(Collider[] hitEnemies) {
        if (currentEnemy != null && Vector3.Distance(transform.position, currentEnemy.position) <= attackRange)
            return;
        if (hitEnemies.Length > 0) {
            int randomIndex = Random.Range(0, hitEnemies.Length);
            currentEnemy = hitEnemies[randomIndex].transform;
        }
        else {
            currentEnemy = null;
        }
    }

    // Rotate the tower head towards the current enemy
    protected virtual void RotateTowardsEnemy() {
        if (currentEnemy == null) return;

        Vector3 direction = currentEnemy.position - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerHead.rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Draw attack range gizmo in the editor for visual representation
    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint) {
        if (currentEnemy == null) return Vector3.zero;
        return (currentEnemy.position - startPoint.position).normalized;
    }
}