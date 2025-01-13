using UnityEngine;
using UnityEngine.SceneManagement;
using static Tower;

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
    [SerializeField] private int gold;
    private UIScript hudScript;

    //przeniesonie z TowerSpawner
    private GameObject pendingTower; // Tymczasowa wie¿a w trakcie ustawiania
    private Camera mainCamera; // Kamera do raycastów
    [SerializeField] private GridManager gridManager; // Referencja do mened¿era siatki

    private void Start()
    {
        mainCamera = Camera.main;
        hudScript = FindFirstObjectByType<UIScript>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not assigned! Please attach it in the inspector.");
        }
        ChangeGold(0);
    }

    void Update()
    {
        SelectTurret();
        DebugControls();

    }
    
    //wywo³uje umierajacy wrog
    public void ChangeGold(int value)
    {
        gold += value;
        hudScript.UpdateGold(gold);
    }
    public bool EnoughGold(GameObject tower)
    {
        int cost = tower.GetComponent<Tower>().getCost();
        if (cost <= gold) { 
            ChangeGold(-cost);
            return true;  }
        else { Debug.Log("false"); return false;}
        
    }
    public void DebugControls()
    {
        // Zmiana miejsca bazy
        //if (Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        target.transform.position = new Vector3(hit.point.x, 1, hit.point.z);
        //    }
        //}

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
    public void SelectTurret()
    {
        
        // Rozpoczêcie stawiania ró¿nych typów wie¿
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerWind);
                hudScript.activeTurret("wind");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerWater);
                hudScript.activeTurret("water");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerFire);
                hudScript.activeTurret("fire");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(towerEarth);
                hudScript.activeTurret("earth");
            }
            
        }
        
        // Rozpoczêcie stawiania barykady
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (pendingTower == null)
            {
                pendingTower = Instantiate(barricade);
                hudScript.activeTurret("barricade");
            }
            
        }

            UpdatePlacement();
        
    }

    public void StartPlacingTower(GameObject towerPrefab)
    {
        // Tworzymy now¹ wie¿ê tylko jeœli ¿adna nie jest ju¿ ustawiana
        if (pendingTower == null)
        {
            pendingTower = Instantiate(towerPrefab);
        }
    }

    public void UpdatePlacement()
    {
        if (pendingTower == null || gridManager == null) return;

        // Raycast w celu okreœlenia pozycji myszy w œwiecie
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector2Int gridPosition = gridManager.GetGridCoordinates(raycastHit.point);

            // Sprawdzanie, czy komórka jest dostêpna
            if (gridManager.IsCellAvailable(gridPosition.x, gridPosition.y) )
            {
                Vector3 worldPosition = gridManager.GetWorldPosition(gridPosition.x, gridPosition.y);
                pendingTower.transform.position = new Vector3(worldPosition.x, 0.5f, worldPosition.z);
            }
        }
        if (pendingTower.GetComponent<Tower>().canPlace)
        {
            if (Input.GetMouseButtonDown(0) && EnoughGold(pendingTower))
            {
                PlaceTower();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(pendingTower);
            pendingTower = null;
            hudScript.cancelActiveTurret();
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
                if (pendingTower.GetComponent<Tower>().towerType != TowerType.barricade)
                {
                    gridManager.PlaceTower(gridPosition.x, gridPosition.y); // Oznacz komórkê jako zajêt¹
                }
                pendingTower = null; // Koñczymy tryb ustawiania
                hudScript.cancelActiveTurret();
            }
            else
            {
                Debug.LogWarning("Cannot place tower here.");
            }
        }
    }
}
