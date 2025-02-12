using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Tower;
//using static Unity.VisualScripting.Round<TInput, TOutput>;
//using static Unity.VisualScripting.Round<TInput, TOutput>;

public class gameControlScript : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject barricade;
    [SerializeField] private GameObject towerFire;
    [SerializeField] private GameObject towerWater;
    [SerializeField] private GameObject towerEarth;
    [SerializeField] private GameObject towerWind;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private int gold;
    [SerializeField] private int health;
    [SerializeField] private bool roundPlaying;
    private int enemiesAlive;
    private UIScript hudScript;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private GameObject pendingTower;
    private Camera mainCamera;
    [SerializeField] private GridManager gridManager;

    private void Start()
    {
        
        roundPlaying = false;
        mainCamera = Camera.main;
        hudScript = FindFirstObjectByType<UIScript>();
        ChangeGold(0);
        ChangeHealth(0);
    }

    void Update()
    {
        SelectTurret();
        DebugControls();
        GameOver();
    }
    public void RoundStart()
    {
        roundPlaying = true;
        enemySpawner.SpawnWave();
        hudScript.RoundButtonActive(false);
    }
    public void CheckForRound(int enemies)
    {
        enemiesAlive+= enemies;
        Debug.Log(enemiesAlive);
        if (enemiesAlive < 1)
        {
            roundPlaying = false;
            hudScript.RoundButtonActive(true);
        }
        Debug.Log(roundPlaying);
    }

    public void ChangeGold(int value)
    {
        gold += value;
        hudScript.UpdateGold(gold);
    }
    public void ChangeHealth(int value)
    {
        health -= value;
        hudScript.UpdateHealth(health);
    }
    public void GameOver()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene("menu");
        }
    }
    public bool EnoughGold(GameObject tower)
    {
        int cost = tower.GetComponent<Tower>().getCost();
        if (cost <= gold) { 
            ChangeGold(-cost);
            return true;  }
        else { Debug.Log("za malo siana"); return false;}
        
    }
    public void DebugControls()
    {
        //if (Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        target.transform.position = new Vector3(hit.point.x, 1, hit.point.z);
        //    }
        //}

        if (Input.GetKeyUp(KeyCode.S))
        {
            roundPlaying = true;
            enemySpawner.SpawnEnemy();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            roundPlaying=true;
            enemySpawner.SpawnWave();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void SelectTurret()
    {
        
        
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
        
        if (Input.GetKeyDown(KeyCode.Alpha5) && !roundPlaying)
        {
            if (pendingTower == null )
            {
                pendingTower = Instantiate(barricade);
                hudScript.activeTurret("barricade");
            }
            
        }

            UpdatePlacement();
        
    }

    public void StartPlacingTower(GameObject towerPrefab)
    {
        if (pendingTower == null)
        {
            pendingTower = Instantiate(towerPrefab);
        }
    }

    public void UpdatePlacement()
    {
        if (pendingTower == null || gridManager == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector2Int gridPosition = gridManager.GetGridCoordinates(raycastHit.point);

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
                pendingTower.GetComponent<Tower>().towerActivated = true;
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
                    gridManager.PlaceTower(gridPosition.x, gridPosition.y);
                }
                pendingTower = null;
                hudScript.cancelActiveTurret();
            }
            else
            {
                Debug.LogWarning("Nue mozesz tu postawic");
            }
        }
    }
}
