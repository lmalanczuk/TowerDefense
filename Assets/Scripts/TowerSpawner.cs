using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject towerPrefab; // Prefab wie�y
    private GameObject pendingTower; // Tymczasowa wie�a w trakcie ustawiania
    private Camera mainCamera; // Kamera do raycast�w

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartPlacingTower(GameObject towerPrefab)
    {
        // Je�li nie ma ju� oczekuj�cej wie�y, tworzona jest nowa
        if (pendingTower == null)
        {
            pendingTower = Instantiate(towerPrefab); // Tworzymy now� wie��
            pendingTower.SetActive(true); // Upewniamy si�, �e wie�a jest widoczna
        }
    }

    public void UpdatePlacement()
    {
        if (pendingTower == null) return;

        // Przesuwanie wie�y na podstawie pozycji myszy
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pendingTower.transform.position = new Vector3(hit.point.x, 0.5f, hit.point.z); // Wie�a pod��a za kursorem myszy
        }

        // Zatwierdzenie ustawienia wie�y
        if (Input.GetMouseButtonDown(1)) // Prawy przycisk myszy
        {
            // Ustalamy miejsce postawienia wie�y
            pendingTower = null; // Ko�czymy tryb ustawiania, mo�emy stawia� kolejn� wie��
        }
    }
}
