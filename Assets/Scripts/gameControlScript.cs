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
    [SerializeField] private EnemySpawner enemySpawner; // Referencja do spawnera
    [SerializeField] private int gold = 0;
    private UIScript hudScript;

    //przeniesonie z TowerSpawner
    private GameObject pendingTower; // Tymczasowa wie�a w trakcie ustawiania
    private Camera mainCamera; // Kamera do raycast�w
    [SerializeField] private GridManager gridManager; // Referencja do mened�era siatki

    private void Start()
    {
        mainCamera = Camera.main;
        hudScript = FindFirstObjectByType<UIScript>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not assigned! Please attach it in the inspector.");
        }
    }

    void Update()
    {
        SelectTurret();
        DebugControls();

    }
    
    //wywo�uje umierajacy wrog
    public void ChangeGold(int value)
    {
        gold += value;
        hudScript.UpdateGold(gold);
    }
    public void DebugControls()
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

        // Spawnowanie wrog�w
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
    public void SelectTurret()
    {
        
        // Rozpocz�cie stawiania r�nych typ�w wie�
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerWind);
                hudScript.activeTurret("wind");
                pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerWater);
                hudScript.activeTurret("water");
                pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerFire);
                hudScript.activeTurret("fire");
                pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerEarth);
                hudScript.activeTurret("earth");
                pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
            }
            
        }
        
        // Rozpocz�cie stawiania barykady
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(barricade);
                hudScript.activeTurret("barricade");
                pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
            }
            
        }

        // Aktualizacja lokalizacji wie�y
        UpdatePlacement();
    }

    public void StartPlacingTower(GameObject towerPrefab)
    {
        // Tworzymy now� wie�� tylko je�li �adna nie jest ju� ustawiana
        if (pendingTower == null)
        {
            pendingTower = Instantiate(towerPrefab);
            pendingTower.SetActive(false); // Tymczasowa wie�a jest niewidoczna, dop�ki nie znajdzie miejsca
        }
    }

    public void UpdatePlacement()
    {
        if (pendingTower == null || gridManager == null) return;

        // Raycast w celu okre�lenia pozycji myszy w �wiecie
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector2Int gridPosition = gridManager.GetGridCoordinates(raycastHit.point);

            // Sprawdzanie, czy kom�rka jest dost�pna
            if (gridManager.IsCellAvailable(gridPosition.x, gridPosition.y))
            {
                Vector3 worldPosition = gridManager.GetWorldPosition(gridPosition.x, gridPosition.y);
                pendingTower.transform.position = new Vector3(worldPosition.x, 0.5f, worldPosition.z);
                pendingTower.SetActive(true); // Wie�a staje si� widoczna
            }
            else
            {
                pendingTower.SetActive(false); // Ukryj wie��, je�li nie ma miejsca
            }
        }

        // Zatwierdzenie ustawienia wie�y po klikni�ciu prawym przyciskiem myszy
        if (Input.GetMouseButtonDown(1))
        {
            PlaceTower();
        }
    }

    private void PlaceTower()
    {
        if (pendingTower == null || gridManager == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector2Int gridPosition = gridManager.GetGridCoordinates(raycastHit.point);
            if (gridManager.IsCellAvailable(gridPosition.x, gridPosition.y))
            {
                gridManager.PlaceTower(gridPosition.x, gridPosition.y); // Oznacz kom�rk� jako zaj�t�
                pendingTower = null; // Ko�czymy tryb ustawiania
                hudScript.cancelActiveTurret();
            }
            else
            {
                Debug.LogWarning("Cannot place tower here.");
            }
        }
    }
}
