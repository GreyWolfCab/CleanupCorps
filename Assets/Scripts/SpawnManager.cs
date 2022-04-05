using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Enemies")]
    [SerializeField] private GameObject enemyPrefab;
    private List<GameObject> enemyList;
    private int currentEnemy;//current enemy to spawn
    private int enemiesRemaining;//enemies left to kill before next wave
    private Vector2 enemySpeedRange = new Vector2(25f, 50f);
    private float slowChance = 0.4f;
    private float fastChance = 0.05f;
    private float rateChanceChange = 0.01f;
    private GameObject enemyHolder;//parent gameobject for all enemies

    [Header("Spawn Point")]
    [SerializeField] private int waveCount = 0;
    [SerializeField] private List<Transform> spawnPoints;
    private GameObject playerObject;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyList = new List<GameObject>();
        enemyHolder = GameObject.Find("EnemyHolder");
        playerObject = GameObject.Find("Player");
        GatherSpawnPoints();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnNewWave();//begin spawning enemies when the scene starts up
    }

    public void CheckEnemiesRemaining() {
        enemiesRemaining = 0;
        foreach (GameObject enemy in enemyList) {
            if (enemy.activeSelf) {
                enemiesRemaining++;
            }
        }
        gameManager.SetEnemyText(enemiesRemaining);//update gui enemies remaining
        if (enemiesRemaining <= 0) {
            SpawnNewWave();
        }
    }

    private void SpawnNewWave() {
        ++waveCount;//iterate wave
        gameManager.SetWaveText(waveCount);//update gui current wave count
        ChangeSpeedRates();//update speed percentages
        while(waveCount > enemyList.Count) {//add enough enemies based on wave count
            AddNewEnemy();//add another enemy to the spawning pool
        }
        currentEnemy = 0;//reset enemy spawn
        InvokeRepeating("SpawnEnemy", 3, 1);//spawn enemies
    }

    private void SpawnEnemy() {
        Vector3 spawnPoint = SelectSpawnPoint().position;
        enemyList[currentEnemy].SetActive(true);
        EnemyMovement enemyMovement = enemyList[currentEnemy].GetComponent<EnemyMovement>();
        enemyList[currentEnemy].GetComponent<EnemyStats>().ResetEnemy();
        enemyMovement.ResetEnemy(spawnPoint);
        int speedChance = Random.Range(0, 100);//percentage choice for enemy speed
        float enemySpeed;
        if (speedChance >= 0 && speedChance < 100 * slowChance) {//slow enemies move speed [25, 30]
            enemySpeed = Random.Range(enemySpeedRange.x, enemySpeedRange.x+5);
        } else if (speedChance >= 100 - (100*fastChance) && speedChance < 100) {//fast enemies move speed [40, 50]
            enemySpeed = Random.Range(enemySpeedRange.y-10, enemySpeedRange.y);
        } else {//normal enemies move speed [30, 40]
            enemySpeed = Random.Range(enemySpeedRange.x + 5f, enemySpeedRange.y);
        }
        enemyMovement.SetMoveSpeed(enemySpeed);//give enemies random speeds in a range
        currentEnemy++;//iterate to next enemy
        enemiesRemaining++;
        gameManager.SetEnemyText(enemiesRemaining);//update gui enemies remaining
        if (currentEnemy >= enemyList.Count) {//stop spawning at enemy limit
            CancelInvoke("SpawnEnemy");
        }
    }

    private void AddNewEnemy() {
        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.parent = enemyHolder.transform;//set enemyHolder as parent to new enemy
        newEnemy.SetActive(false);
        enemyList.Add(newEnemy);
    }

    /*update the percentages affecting the speeds of enemies for this round*/
    private void ChangeSpeedRates() {
        if (slowChance > 0) {
            slowChance -= rateChanceChange;
        }
        if (fastChance < 1) {
            fastChance += rateChanceChange;
        }
    }

    /*
     * Store all possible spawn points in a List for later reference
     */
    private void GatherSpawnPoints() {
        spawnPoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++) {
            spawnPoints.Add(transform.GetChild(i));
        }
    }

    /*
     * Finds the spawn point currently closest to the player
     */
    private Transform SelectSpawnPoint() {
        Transform nearestSpawnPoint = null;
        float closestPoint = Mathf.Infinity;
        foreach (Transform point in spawnPoints) {//iterate through all spawn points
            Vector3 newPoint = playerObject.transform.position - point.position;//calculate the difference in player and spawn position
            float distanceSqrd = newPoint.sqrMagnitude;//optimized way of calculating distance
            if (distanceSqrd < closestPoint) {
                closestPoint = distanceSqrd;//set new closest point
                nearestSpawnPoint = point;// set new possible spawn point
            }
        }

        return nearestSpawnPoint;
    }
}
