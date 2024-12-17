using UnityEngine;
using UnityEngine.SceneManagement;

public class gameControlScript : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject barricade;
    [SerializeField] private GameObject towerFire;
    [SerializeField] private GameObject towerWater;
    [SerializeField] private GameObject towerEarth;
    [SerializeField] private GameObject towerWind;
    [SerializeField] private TowerSpawner towerSpawner; // Odwo³anie do TowerSpawner
    [SerializeField] private EnemySpawner enemySpawner; // Referencja do spawnera

    void Update()
    {
        // Zmiana miejsca bazy
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                target.transform.position = new Vector3(hit.point.x, 1, hit.point.z);
            }
        }

        // Rozpoczêcie stawiania ró¿nych typów wie¿
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            towerSpawner.StartPlacingTower(towerFire);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            towerSpawner.StartPlacingTower(towerWater);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            towerSpawner.StartPlacingTower(towerEarth);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            towerSpawner.StartPlacingTower(towerWind);
        }
        // Rozpoczêcie stawiania barykady
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            towerSpawner.StartPlacingTower(barricade);
        }

        // Aktualizacja lokalizacji wie¿y
        towerSpawner.UpdatePlacement();

        // Spawnowanie wrogów
        if (Input.GetKeyUp(KeyCode.S))
        {
            enemySpawner.SpawnEnemy();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            enemySpawner.SpawnWave();
        }

        // Restart
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
