using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    private GameObject pendingTower; // Tymczasowa wie�a w trakcie ustawiania
    private Camera mainCamera; // Kamera do raycast�w
    [SerializeField] private GridManager gridManager; // Referencja do mened�era siatki

    private void Start()
    {
        mainCamera = Camera.main;

        if (gridManager == null)
        {
            Debug.LogError("GridManager not assigned! Please attach it in the inspector.");
        }
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
            }
            else
            {
                Debug.LogWarning("Cannot place tower here.");
            }
        }
    }
}
