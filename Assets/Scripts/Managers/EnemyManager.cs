using UnityEngine;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private Transform respawn;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyPrefab;

    public float spawnCooldown; // Time between enemy spawns
    public float spawnTimer; // Time until next enemy spawn

    // Reduces spawnTimer by DeltaTime every frame then creates an enemy when spawnTimer reaches 0
    private void Update() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0) {
            CreateEnemy();
        }
    }

    // Instantiates an enemy at the respawn point and resets the spawnTimer
    private void CreateEnemy() {
        GameObject newEnemy = Instantiate(enemyPrefab, respawn.position, Quaternion.identity);
        spawnTimer = spawnCooldown;
    }
}
