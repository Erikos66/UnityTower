using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDetails {
    public int basicEnemy; // Number of basic enemies to spawn
    public int fastEnemy; // Number of fast enemies to spawn
}
public class EnemyManager : MonoBehaviour {

    public float spawnCooldown; // Time between enemy spawns
    public float spawnTimer; // Time until next enemy spawn

    [Header("Wave Control")]
    [SerializeField] private WaveDetails waves; // Number of enemies to spawn in each wave
    [SerializeField] private Transform respawn; // Position to spawn enemies
    private List<GameObject> enemiesToCreate; // List of enemies to spawn


    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy; // Prefab for basic enemy
    [SerializeField] private GameObject fastEnemy; // Prefab for fast enemy

    // Creates a new list of enemies when the game starts
    private void Start() {
        enemiesToCreate = NewEnemyWave();
    }

    // Reduces spawnTimer by DeltaTime every frame then creates an enemy when spawnTimer reaches 0
    private void Update() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && enemiesToCreate.Count > 0) {
            CreateEnemy();
            spawnTimer = spawnCooldown;
        }
    }

    // Instantiates an enemy at the respawn point and resets the spawnTimer
    private void CreateEnemy() {
        GameObject randomEnemy = GetRandomEnemy();
        GameObject newEnemy = Instantiate(randomEnemy, respawn.position, Quaternion.identity);
    }

    // Returns a random enemy from the list of enemies to create
    private GameObject GetRandomEnemy() {
        int randomIndex = Random.Range(0, enemiesToCreate.Count);
        GameObject chosenEnemy = enemiesToCreate[randomIndex];
        enemiesToCreate.RemoveAt(randomIndex);
        return chosenEnemy;
    }

    // Creates a new list of enemies based on the wave details
    private List<GameObject> NewEnemyWave() {
        List<GameObject> newEnemylist = new List<GameObject>();

        for (int i = 0; i < waves.basicEnemy; i++) {
            newEnemylist.Add(basicEnemy);
        }

        for (int i = 0; i < waves.fastEnemy; i++) {
            newEnemylist.Add(fastEnemy);
        }

        return newEnemylist;
    }
}
