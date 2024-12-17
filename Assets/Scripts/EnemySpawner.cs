using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject lightEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private int wave = 1;
    private float lightChance = 1;
    private float heavyChance = 0;
    private int enemiesNumber = 3;
    private float spawnDelay = 0.5f;

    // Ta metoda jest odpowiedzialna za spawnowanie wrogów
    public void SpawnEnemy()
    {
        int spawnPointIndex = Random.Range(0, 2); // Losowanie punktu spawnu
        Transform spawnPoint = (spawnPointIndex == 0) ? spawnPoint1 : spawnPoint2;

        int enemyClass = Random.Range(0, 2); // Losowanie klasy wroga (0 - Light, 1 - Heavy)
        GameObject enemyPrefab = (enemyClass == 0) ? lightEnemyPrefab : heavyEnemyPrefab;

        // Spawnowanie wroga w wybranym punkcie
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    public void SpawnChances()
    {
        enemiesNumber = Mathf.CeilToInt(Mathf.Pow(wave, 1.2f) + wave + 2);
        if (wave > 1 &&  wave < 6)
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
            yield return new WaitForSeconds(spawnDelay); 
        }
    }
}
