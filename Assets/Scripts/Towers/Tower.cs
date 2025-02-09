using System;
using UnityEngine;

public class Tower : MonoBehaviour {

    public Transform currentEnemy; // Current enemy the tower is targeting

    [Header("Tower Setup")]
    [SerializeField] private float range; // Range of the tower
    [SerializeField] private Transform towerHead; // Head of the tower
    [SerializeField] private float rotationSpeed; // Speed at which the tower head rotates

    private void Update() {
        RotateTowardsEnemy();
    }

    // Rotations the head of the tower towards the current enemy using Lerp
    private void RotateTowardsEnemy() {
        Vector3 directionToEnemy = currentEnemy.position - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;

        towerHead.rotation = Quaternion.Euler(rotation);
    }
}
