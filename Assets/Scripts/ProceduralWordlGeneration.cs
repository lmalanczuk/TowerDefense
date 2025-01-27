using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.VersionControl;

public class ProceduralWorldGenerator : MonoBehaviour
{
    public int mapWidth = 20;
    public int mapHeight = 20;
    public GameObject groundPrefab;
    public GameObject obstaclePrefab;
    public GameObject enemySpawnPointPrefab;
    public GameObject playerBasePrefab;
    public GameObject edgeObstaclePrefab;
    public GameObject fenceGroundPrefab;
    public GameObject cornerFencePrefab; // Prefab dla estetycznych rogów płotu

    public Transform parentContainer;
    public NavMeshSurface navMeshSurface;
    public NavMeshSurface navMeshSurfaceHeavy;// Referencja do komponentu NavMeshSurface

    private Vector2Int enemySpawnPoint1;
    private Vector2Int enemySpawnPoint2;
    private Vector2Int playerBase;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond); // Dodanie ziarna losowości

        playerBase = GetRandomEdgePosition(); // Najpierw tworzymy bazę gracza
        enemySpawnPoint1 = GetRandomEdgePosition(playerBase, 12); // Tworzymy pierwszy spawn wroga, sprawdzając odległość od bazy
        enemySpawnPoint2 = GetRandomEdgePosition(playerBase, 12, enemySpawnPoint1, 8); // Tworzymy drugi spawn wroga z odpowiednią odległością od bazy i pierwszego spawnera

        HashSet<Vector2Int> allPaths = GeneratePaths(3);

        GenerateGround();
        GenerateFences();
        GenerateObstacles(allPaths);
        PlaceEnemySpawnPoints();
        PlacePlayerBase();

        UpdateNavMesh(); // Aktualizacja NavMesha po wygenerowaniu mapy
    }

    void GenerateFences()
    {
        for (int x = -1; x <= mapWidth; x++)
        {
            for (int z = -1; z <= mapHeight; z++)
            {
                bool isEdge = (x == -1 || z == -1 || x == mapWidth || z == mapHeight);
                bool isCorner = (x == -1 && z == -1) || (x == -1 && z == mapHeight) ||
                                (x == mapWidth && z == -1) || (x == mapWidth && z == mapHeight);

                Vector3 position = new Vector3(x + 0.5f, 0.1f, z + 0.5f);
                if (isCorner)
                {
                    Instantiate(cornerFencePrefab, position, Quaternion.identity, transform);
                }
                else if (isEdge)
                {
                    Quaternion rotation = (z == -1 || z == mapHeight) ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
                    Instantiate(fenceGroundPrefab, position, rotation, transform);
                    Instantiate(edgeObstaclePrefab, position, rotation, transform);
                }
            }
        }
    }

    void GenerateGround()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector3 position = new Vector3(x + 0.5f, 0, z + 0.5f);
                Instantiate(groundPrefab, position, Quaternion.identity, transform);
            }
        }
    }

    void GenerateObstacles(HashSet<Vector2Int> paths)
    {
        HashSet<Vector2Int> safeZone = GetSafeZone(enemySpawnPoint1, 2); // Strefa bezpieczeństwa dla pierwszego spawnera wroga
        safeZone.UnionWith(GetSafeZone(enemySpawnPoint2, 2)); // Strefa bezpieczeństwa dla drugiego spawnera wroga
        safeZone.UnionWith(GetSafeZone(playerBase, 2)); // Strefa bezpieczeństwa dla bazy gracza

        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int z = 1; z < mapHeight - 1; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);
                if (paths.Contains(pos) || safeZone.Contains(pos)) continue;

                float perlinValue = Mathf.PerlinNoise((x + Random.Range(0, 100)) * 0.1f, (z + Random.Range(0, 100)) * 0.1f); // Losowość w szumie Perlina
                if (perlinValue > 0.55f)
                {
                    Vector3 position = new Vector3(x + 0.5f, 0.5f, z + 0.5f);
                    Instantiate(obstaclePrefab, position, Quaternion.identity, parentContainer);
                }
            }
        }
    }

    HashSet<Vector2Int> GeneratePaths(int pathCount)
    {
        HashSet<Vector2Int> allPaths = new HashSet<Vector2Int>();

        for (int i = 0; i < pathCount; i++)
        {
            HashSet<Vector2Int> path = GenerateSinglePath();
            allPaths.UnionWith(path);
        }

        foreach (var pos in allPaths)
        {
            Vector3 position = new Vector3(pos.x + 0.5f, 0f, pos.y + 0.5f);
            Instantiate(fenceGroundPrefab, position, Quaternion.identity, transform); // Wizualizacja ścieżek
        }

        return allPaths;
    }

    HashSet<Vector2Int> GenerateSinglePath()
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        Vector2Int currentPos = enemySpawnPoint1;
        Vector2Int endPos = playerBase;

        path.Add(currentPos);

        while (currentPos != endPos)
        {
            List<Vector2Int> possibleMoves = GetNeighbors(currentPos);
            possibleMoves.RemoveAll(n => path.Contains(n) || IsEdge(n));

            if (possibleMoves.Count > 0)
            {
                currentPos = possibleMoves[Random.Range(0, possibleMoves.Count)];
                path.Add(currentPos);
            }
            else
            {
                break;
            }
        }

        return path;
    }

    List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(current.x + 1, current.y),
            new Vector2Int(current.x - 1, current.y),
            new Vector2Int(current.x, current.y + 1),
            new Vector2Int(current.x, current.y - 1)
        };

        neighbors.RemoveAll(n => n.x < 0 || n.x >= mapWidth || n.y < 0 || n.y >= mapHeight);
        return neighbors;
    }

    Vector2Int GetRandomEdgePosition(Vector2Int? exclude = null, int minDistance = 0, Vector2Int? exclude2 = null, int minDistance2 = 0)
    {
        Vector2Int position;
        do
        {
            int side = Random.Range(0, 4);
            switch (side)
            {
                case 0: position = new Vector2Int(0, Random.Range(0, mapHeight)); break; // Lewa krawędź
                case 1: position = new Vector2Int(mapWidth - 1, Random.Range(0, mapHeight)); break; // Prawa krawędź
                case 2: position = new Vector2Int(Random.Range(0, mapWidth), 0); break; // Dolna krawędź
                default: position = new Vector2Int(Random.Range(0, mapWidth), mapHeight - 1); break; // Górna krawędź
            }
        } while ((exclude.HasValue && Vector2Int.Distance(position, exclude.Value) < minDistance) ||
                 (exclude2.HasValue && Vector2Int.Distance(position, exclude2.Value) < minDistance2));

        return position;
    }

    HashSet<Vector2Int> GetSafeZone(Vector2Int center, int radius)
    {
        HashSet<Vector2Int> safeZone = new HashSet<Vector2Int>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int pos = new Vector2Int(center.x + x, center.y + z);
                if (pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight)
                {
                    safeZone.Add(pos);
                }
            }
        }

        return safeZone;
    }

    bool IsEdge(Vector2Int pos)
    {
        return pos.x == 0 || pos.x == mapWidth - 1 || pos.y == 0 || pos.y == mapHeight - 1;
    }

    void PlaceEnemySpawnPoints()
    {
        Vector3 position1 = new Vector3(enemySpawnPoint1.x + 0.5f, 0.5f, enemySpawnPoint1.y + 0.5f);
        Vector3 position2 = new Vector3(enemySpawnPoint2.x + 0.5f, 0.5f, enemySpawnPoint2.y + 0.5f);
        Instantiate(enemySpawnPointPrefab, position1, Quaternion.identity, transform);
        Instantiate(enemySpawnPointPrefab, position2, Quaternion.identity, transform);
    }

    void PlacePlayerBase()
    {
        Vector3 position = new Vector3(playerBase.x + 0.5f, 0.5f, playerBase.y + 0.5f);
        Instantiate(playerBasePrefab, position, Quaternion.identity, transform);
    }

    void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            navMeshSurfaceHeavy.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("NavMeshSurface is not assigned. Please assign it in the Inspector.");
        }
    }
}