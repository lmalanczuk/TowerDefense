using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject lightEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

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
}
