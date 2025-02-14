using UnityEngine;
using UnityEngine.Timeline;

// This class defines the behavior for the Tower_Crossbow,
// handling targeting, shooting via raycasts, and triggering visual effects.
public class Tower_Crossbow : Tower {
    private Tower_Crossbow_Visuals visuals; // Reference to the Tower_Crossbow_Visuals component to control visual effects.

    [Header("Crossbow Settings")]
    [SerializeField] private Transform gunPoint; // Transform representing the gun point from where the raycast is fired.

    /// <summary>
    /// Unity Awake method for caching the visuals component.
    /// </summary>
    protected override void Awake() {
        visuals = GetComponent<Tower_Crossbow_Visuals>(); // Retrieve and cache the visual effects component.
    }

    /// <summary>
    /// Overrides the base Attack method to perform a raycast attack, rotate the tower head,
    /// and trigger both shooting and reload visual effects.
    /// </summary>
    protected override void Attack() {
        base.Attack(); // Execute base attack behavior.
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint); // Calculate direction from gunPoint to the enemy.

        // Perform a raycast to detect if an enemy is hit.
        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, attackRange, LayerMask.GetMask("Enemy"))) {
            towerHead.forward = directionToEnemy; // Rotate the tower head to face the enemy.
            if (hitInfo.transform == currentEnemy) { // Check if the detected enemy is the current target.
                Debug.DrawRay(gunPoint.position, directionToEnemy * attackRange, Color.red, 1f); // Debug visual: draw ray.
                Debug.DrawRay(gunPoint.position, directionToEnemy * attackRange, Color.red, 1f); // (Optional duplicate for debugging.)
                visuals.PlayAttackVFX(gunPoint.position, hitInfo.point); // Trigger the attack visual effects.
                visuals.ReloadDurationVFX(attackCooldown); // Trigger the reload visual effects based on attack cooldown.
            }
        }
    }
}