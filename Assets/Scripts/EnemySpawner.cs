using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject lightEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] public int wave = 1;
    private float lightChance = 1;
    private float heavyChance = 0;
    private int enemiesNumber = 3;
    private float spawnDelay = 0.5f;
    public int enemiesAlive;

    private void Start()
    {
        Transform[] spawnPoints = FindObjectsOfType<Transform>();
        List<Transform> spawnPointList = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point.name == "SpawnPoint")
            {
                spawnPointList.Add(point);
            }
        }

        if (spawnPointList.Count >= 2)
        {
            spawnPoint1 = spawnPointList[0];
            spawnPoint2 = spawnPointList[1];
        }
        else
        {
            Debug.LogError("Nie znaleziono dwóch obiektów SpawnPoint1 w scenie.");
        }
    }
    
    public void SpawnEnemy()
    {
        int spawnPointIndex = Random.Range(0, 2);
        Transform spawnPoint = (spawnPointIndex == 0) ? spawnPoint1 : spawnPoint2;

        int enemyClass = Random.Range(0, 2);
        GameObject enemyPrefab = (enemyClass == 0) ? lightEnemyPrefab : heavyEnemyPrefab;

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        FindFirstObjectByType<gameControlScript>().CheckForRound(1);
        
    }

    public void SpawnChances()
    {
        enemiesNumber = Mathf.CeilToInt(Mathf.Pow(wave, 1.2f) + wave + 2);
        if (wave > 1 && wave < 6)
        {
            heavyChance = heavyChance + 0.05f;
            lightChance = lightChance - 0.05f;
        }

    }
    public void SpawnWave()
    {
        SpawnChances();
        Debug.Log($"Spawning Wave {wave} with {enemiesNumber} enemies.\n" +
                  $"Heavy {heavyChance}, Light {lightChance}");
        StartCoroutine(SpawnEnemiesWithDelay());

        wave++;
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        for (int i = 0; i < enemiesNumber; i++)
        {
            int spawnPointIndex = Random.Range(0, 2);
            Transform spawnPoint = (spawnPointIndex == 0) ? spawnPoint1 : spawnPoint2;

            float randomValue = Random.value;
            GameObject enemyPrefab = (randomValue <= lightChance) ? lightEnemyPrefab : heavyEnemyPrefab;

            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            FindFirstObjectByType<gameControlScript>().CheckForRound(1);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}