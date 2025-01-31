using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    public TowerType towerType;
    [SerializeField]
    private GameObject bulletPrefab;
    private float attackRange;
    private float attackCooldown;
    [SerializeField]
    private LayerMask enemyLayer;
    private float towerHeight = 2f;
    private float bulletForce;
    private int cost;
    public bool canPlace;
    public bool towerActivated;
    private Renderer objectRenderer;
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();




    private float timeSinceLastAttack = 0f;

    private void Start()
    {
        SetType();
        if (towerType != TowerType.barricade)
        {
            makeTransparent();
        }
    }
    public enum TowerType
    {
        earth,
        fire,
        water,
        wind,
        barricade
    }
    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0 && towerActivated)
        {
            GameObject targetEnemy = enemiesInRange[0].gameObject;

            if (timeSinceLastAttack >= attackCooldown)
            {
                ShootAtEnemy(targetEnemy);
                timeSinceLastAttack = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            if(towerType == TowerType.barricade)
            {
                makeTransparent();
                canPlace = false;
            }
            else
            {
                RestoreOriginalMaterials();
                canPlace = true;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            if (towerType == TowerType.barricade)
            {
                RestoreOriginalMaterials();
                canPlace = true;
            }
            else
            {
                makeTransparent();
                canPlace = false;
            }
        }
    }

    private void SaveOriginalMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            originalMaterials[renderer] = renderer.materials;
        }
    }
    public void makeTransparent()
    {
        SaveOriginalMaterials();

        foreach (Renderer renderer in originalMaterials.Keys)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                Material redTransparentMaterial = new Material(renderer.materials[i]);
                redTransparentMaterial.color = new Color(1f, 0f, 0f, 0.5f);
                redTransparentMaterial.SetFloat("_Mode", 3); 
                redTransparentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                redTransparentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                redTransparentMaterial.SetInt("_ZWrite", 0);
                redTransparentMaterial.DisableKeyword("_ALPHATEST_ON");
                redTransparentMaterial.EnableKeyword("_ALPHABLEND_ON");
                redTransparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                redTransparentMaterial.renderQueue = 3000;

                newMaterials[i] = redTransparentMaterial;
            }

            renderer.materials = newMaterials;
        }
    }
    public void RestoreOriginalMaterials()
    {
        foreach (var entry in originalMaterials)
        {
            entry.Key.materials = entry.Value;
        }

        originalMaterials.Clear();
    }
    private void ShootAtEnemy(GameObject enemy)
    {
        Vector3 firePosition = transform.position + Vector3.up * towerHeight;

        GameObject bullet = Instantiate(bulletPrefab, firePosition, Quaternion.identity);

        Vector3 direction = (enemy.transform.position - firePosition).normalized;

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = direction * 10f;

        bulletRb.AddForce(direction * bulletForce, ForceMode.Impulse);
    }

    private void SetType()
    {
        switch (towerType)
        {
            case TowerType.earth:
                attackRange = 5f;
                attackCooldown = 3f;
                bulletForce = 10f;
                cost = 90;
                break;
            case TowerType.fire:
                attackRange = 5f;
                attackCooldown = 2f;
                bulletForce = 10f;
                cost = 50;
                break;
            case TowerType.water:
                attackRange = 5f;
                attackCooldown = 1f;
                bulletForce = 10f;
                cost = 30;
                break;
            case TowerType.wind:
                attackRange = 8f;
                attackCooldown = 1f;
                bulletForce = 13f;
                cost = 15;
                break;
            case TowerType.barricade:
                cost = 60;
                break;
        }
    }

    public int getCost()
    {
        return cost;
    }
}