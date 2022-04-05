using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnManager spawnManager;
    private PlayerStats playerStats;
    private int enemyPrice = 100;
    private Vector3 mapBounds = new Vector3(80, 30, 80);

    [Header("Health")]
    public int totalHealth = 100;
    public int maxHealth = 100;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    private void Update() {
        CheckIfOutOfBounds();
    }

    private void CheckIfOutOfBounds() {
        Vector3 enemyPosition = transform.position;

        if (enemyPosition.x < -mapBounds.x || enemyPosition.x > mapBounds.x) {
            KillEnemy();
        }
        if (enemyPosition.y < -mapBounds.y || enemyPosition.y > mapBounds.y) {
            KillEnemy();
        }
        if (enemyPosition.z < -mapBounds.z || enemyPosition.z > mapBounds.z) {
            KillEnemy();
        }
    }

    public void DamageEnemy(int damage) {
        if (!gameManager.isGameActive) {
            return;
        }
        totalHealth -= damage;
        if (totalHealth <= 0) {
            totalHealth = 0;
            playerStats.AddToBank(enemyPrice);
            KillEnemy();
        }
    }

    private void KillEnemy() {
        gameObject.SetActive(false);
        spawnManager.CheckEnemiesRemaining();
    }

    public void ResetEnemy() {
        totalHealth = 100;
    }
}
