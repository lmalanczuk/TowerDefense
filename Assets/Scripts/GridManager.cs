using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    private GridCell[,] grid;

    void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = GetWorldPosition(x, y);
                grid[x, y] = new GridCell(worldPosition, true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null || Application.isPlaying == false) return;

        Gizmos.color = Color.green;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = GetWorldPosition(x, y);
                Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize + cellSize / 2, 0, y * cellSize + cellSize / 2);
    }

    public bool IsCellAvailable(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false;
        }

        return grid[x, y].IsAvailable;
    }

    public void PlaceTower(int x, int y)
    {
        if (IsCellAvailable(x, y))
        {
            grid[x, y].IsAvailable = false;
        }
    }

    public void RemoveTower(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            grid[x, y].IsAvailable = true;
        }
    }

    public Vector2Int GetGridCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / cellSize);

        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        return new Vector2Int(x, y);
    }
}
