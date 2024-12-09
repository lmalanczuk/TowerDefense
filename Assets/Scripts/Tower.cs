using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private TowerType towerType;
    [SerializeField]
    private GameObject bulletPrefab; // Prefab kuli (pocisku)
    private float attackRange; // Zasięg ataku
    private float attackCooldown; // Czas między strzałami
    [SerializeField]
    private LayerMask enemyLayer; // Warstwa, w której znajdują się przeciwnicy
    private float towerHeight = 2f; // Wysokość wieży, w zależności od jej rozmiaru
    private float bulletForce;



    private float timeSinceLastAttack = 0f; // Czas od ostatniego ataku

    private void Start()
    {
        SetType();
    }
    private enum TowerType
    {
        earth,
        fire,
        water,
        wind
    }
    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        // Wykrywanie wrogów w zasięgu
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            // Strzelaj do pierwszego wykrytego wroga
            GameObject targetEnemy = enemiesInRange[0].gameObject;

            // Jeśli minął czas na nowy strzał, wykonaj atak
            if (timeSinceLastAttack >= attackCooldown)
            {
                ShootAtEnemy(targetEnemy);
                timeSinceLastAttack = 0f; // Resetowanie czasu między atakami
            }
        }
    }

    private void ShootAtEnemy(GameObject enemy)
    {
        // Obliczamy punkt strzału: góra wieży
        Vector3 firePosition = transform.position + Vector3.up * towerHeight;

        // Wystrzeliwujemy pocisk z pozycji na samej górze wieży
        GameObject bullet = Instantiate(bulletPrefab, firePosition, Quaternion.identity);

        // Obliczanie kierunku do wroga
        Vector3 direction = (enemy.transform.position - firePosition).normalized;

        // Uzyskanie Rigidbody pocisku i ustawienie prędkości
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = direction * 10f; // Prędkość pocisku

        // Opcjonalnie: dodanie siły, by pocisk mógł "uderzyć" w cel
        bulletRb.AddForce(direction * bulletForce, ForceMode.Impulse);
    }

    private void SetType()
    {
        switch (towerType)
        {
            case TowerType.earth:
                attackRange = 5f;
                attackCooldown = 5f;
                bulletForce = 30f;
                break;
            case TowerType.fire:
                attackRange = 5f;
                attackCooldown = 1f;
                bulletForce = 10f;
                break;
            case TowerType.water:
                attackRange = 5f;
                attackCooldown = 1f;
                bulletForce = 10f;
                break;
            case TowerType.wind:
                attackRange = 10f;
                attackCooldown = 4f;
                bulletForce = 35f;
                break;
        }
    }
}