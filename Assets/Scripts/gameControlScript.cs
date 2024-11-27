using UnityEngine;
using UnityEngine.SceneManagement;

public class gameControlScript : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private GameObject barricade;
    [SerializeField]
    private GameObject tower;
    [SerializeField]
    private TowerSpawner towerSpawner; // Odwo�anie do TowerSpawner
    [SerializeField]
    private EnemySpawner enemySpawner; // Referencja do spawnera

    private RaycastHit[] hits = new RaycastHit[1];

    void Update()
    {
        // Zmiana miejsca bazy
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
                target.transform.position = new Vector3(hits[0].point.x, 1, hits[0].point.z);
            }
        }

        // Rozpocz�cie stawiania wie�y
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Mo�esz wybra� dowolny przycisk
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
                towerSpawner.StartPlacingTower(tower); // Rozpoczynamy stawianie wie�y
            }
        }

        // Postawienie przeszkody
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
                towerSpawner.StartPlacingTower(barricade); // rozpoczecie stawiania barykady
            }
        }

        // Spawnowanie wrog�w
        if (Input.GetKeyUp(KeyCode.S))
        {
            enemySpawner.SpawnEnemy(); // U�yjemy metody z EnemySpawner do spawnowania wrog�w
        }

        // Restart
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Zaktualizowanie lokalizacji wie�y, je�li jest w trybie ustawiania
        towerSpawner.UpdatePlacement();
    }
}
