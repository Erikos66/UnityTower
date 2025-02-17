using UnityEngine;
using UnityEngine.Timeline;
using System.Collections;

// This class defines the behavior for the Tower_Crossbow,
// handling targeting, shooting via raycasts, and triggering visual effects.
public class Tower_Crossbow : Tower {
    private Tower_Crossbow_Visuals visuals; // Reference to the Tower_Crossbow_Visuals component to control visual effects.

    [Header("Crossbow Settings")]
    [SerializeField] private Transform gunPoint; // Transform representing the gun point from where the raycast is fired.
    [SerializeField] private int TowerDamage = 1; // The amount of damage the tower deals to enemies.

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
        base.Attack();
        StartCoroutine(FireAndTrack());
    }

    // New coroutine to track enemy movement after firing.
    private IEnumerator FireAndTrack() {
        // Continuously update the raycast visual for attackVisualsDuration
        yield return StartCoroutine(visuals.TrackEnemyVFX(gunPoint, currentEnemy.transform, attackCooldown / 2f));
        // After tracking, apply damage (if enemy is still valid).
        if (currentEnemy != null) {
            IDamageable damageable = currentEnemy.GetComponent<IDamageable>();
            damageable?.TakeDamage(TowerDamage);
        }
        // Trigger reload visual effects while keeping rotation enabled.
        visuals.ReloadDurationVFX(attackCooldown);
    }
}