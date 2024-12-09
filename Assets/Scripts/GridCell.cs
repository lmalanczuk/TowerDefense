using UnityEngine;

public class GridCell
{
    public Vector3 WorldPosition { get; private set; }
    public bool IsAvailable { get; set; }

    public GridCell(Vector3 worldPosition, bool isAvailable)
    {
        WorldPosition = worldPosition;
        IsAvailable = isAvailable;
    }
}
