using UnityEngine;
using UnityEngine.Timeline;

public class Tower_Crossbow : Tower {
    [Header("Crossbow Settings")]
    [SerializeField] private Transform gunPoint;


    protected override void Attack() {
        base.Attack();
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, attackRange, LayerMask.GetMask("Enemy"))) {
            towerHead.forward = directionToEnemy;
            if (hitInfo.transform == currentEnemy) {
                Debug.DrawRay(gunPoint.position, directionToEnemy * attackRange, Color.red, 1f);
                // Debug.Log(hitInfo.transform.name + " is hit by " + name);
            }
        }
    }

}
