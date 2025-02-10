using System.Collections.Generic;
using UnityEngine;

public class TowerTargetting : MonoBehaviour {

    public enum TargetMode { Closest, Random } // Targeting modes

    [Header("Targeting Settings")]
    [SerializeField] private TargetMode targetMode = TargetMode.Random; // Default target mode
    [SerializeField] private float attackRange = 3f; // Attack range of the tower
    [SerializeField] private Transform towerHead; // Tower head transform to rotate towards enemy
    [SerializeField] private float rotationSpeed = 5f; // Rotation speed of the tower head

    private Transform currentEnemy;

    private void Update() {
        FindTarget();
        RotateTowardsEnemy();
    }

    // Find the target based on the target mode and assign it to the current enemy
    private void FindTarget() {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        if (targetMode == TargetMode.Closest) {
            FindClosestEnemy(hitEnemies);
        }
        else if (targetMode == TargetMode.Random) {
            FindRandomEnemy(hitEnemies);
        }
    }

    // Find the closest enemy in the attack range and assign it to the current enemy
    private void FindClosestEnemy(Collider[] hitEnemies) {
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
    private void FindRandomEnemy(Collider[] hitEnemies) {
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
    private void RotateTowardsEnemy() {
        if (currentEnemy == null) return;

        Vector3 direction = currentEnemy.position - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerHead.rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Draw attack range gizmo in the editor for visual representation
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}