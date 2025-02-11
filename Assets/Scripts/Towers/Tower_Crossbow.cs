using UnityEngine;
using UnityEngine.Timeline;

public class Tower_Crossbow : Tower {

    private Tower_Crossbow_Visuals visuals; // Reference to the Tower_Crossbow_Visuals script

    [Header("Crossbow Settings")]
    [SerializeField] private Transform gunPoint; // The point from which the raycast is fired

    protected override void Awake() {
        visuals = GetComponent<Tower_Crossbow_Visuals>(); // Get the Tower_Crossbow_Visuals script component
    }


    protected override void Attack() {
        base.Attack(); // Call the base Attack method
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint); // Get the direction to the current enemy

        // Check if there is an enemy in the direction of the raycast
        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, attackRange, LayerMask.GetMask("Enemy"))) {
            // If the raycast hits the current enemy, the tower head should face the enemy
            towerHead.forward = directionToEnemy;
            // If the raycast hits the current enemy, deal damage to it
            if (hitInfo.transform == currentEnemy) {
                Debug.DrawRay(gunPoint.position, directionToEnemy * attackRange, Color.red, 1f);
                // Uncomment line below to see the raycast hits in the editor
                // Debug.Log(hitInfo.transform.name + " is hit by " + name);
                Debug.DrawRay(gunPoint.position, directionToEnemy * attackRange, Color.red, 1f);
                visuals.PlayAttackVFX(gunPoint.position, hitInfo.point);
            }
        }
    }
}