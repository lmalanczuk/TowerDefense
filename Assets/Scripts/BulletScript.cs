using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private BulletType bulletType;
    private int damage;
    private float explosionRadius; // Zasiêg eksplozji dla proximity damage
    private ParticleSystem particleSystem;
    private ParticleSystem impactParticles;

    private enum BulletType
    {
        earth,
        fire,
        water,
        wind
    }

    void Start()
    {
        SetType(); 
        particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
        impactParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
      
            DealDamage(other.gameObject);
            HandleParticles();
            Destroy(gameObject);
        }
    }

    private void ApplyProximityDamage()
    {
        // ZnajdŸ wszystkich wrogów w zasiêgu eksplozji z warstw¹ "Enemies"
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Enemies"));

        foreach (var collider in enemiesInRange)
        {
            // Dodatkowo upewnij siê, ¿e collider ma tag "Enemy"
            if (collider.CompareTag("Enemy"))
            {
                EnemyScript enemy = collider.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    enemy.Health -= damage;
                }
            }
        }
    }


    private void DealDamage(GameObject target)
    {
        EnemyScript enemy = target.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.Health -= damage;
            switch (bulletType)
            {
                case BulletType.fire:
                    enemy.ApplyBurn();
                    break;
                case BulletType.water:
                    enemy.ApplySlow();
                    break;
                case BulletType.earth:
                    ApplyProximityDamage();
                    break;
                case BulletType.wind:
                    
                    break;
            }
            ImpactParticles();
        }
    }

    private void ImpactParticles()
    {
        impactParticles.Play();
        impactParticles.transform.parent = null;
    }
    private void HandleParticles()
    {
        particleSystem.transform.parent = null;
        particleSystem.emissionRate = 0;
    }

    private void SetType()
    {
        switch (bulletType)
        {
            case BulletType.earth:
                damage = 20;
                explosionRadius = 5f;
                break;
            case BulletType.fire:
                damage = 25;
                break;
            case BulletType.water:
                damage = 10;
                break;
            case BulletType.wind:
                damage = 20;
                break;
        }
    }
}
