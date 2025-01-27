using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScript : MonoBehaviour
{
    private GameObject target;
    private NavMeshAgent agent;

    private int hp;
    public int Health
    {
        get { return hp; }
        set { hp = value; }
    }

    private float ms;
    private int reward;
    private int damage;

    [SerializeField]
    private EnemyClass enemyClass;
    public enum EnemyClass
    {
        Light,
        Heavy,
    }

    private ParticleSystem waterParticles;
    private bool isSlowed;
    private Coroutine slowCoroutine;

    private ParticleSystem fireParticles;
    private bool isBurning;
    private Coroutine burnCoroutine;

    private List<GameObject> barricades;
    private GameObject currentTargetBarricade;

    private gameControlScript gameControlScript;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            gameControlScript.ChangeHealth(damage);
            gameControlScript.CheckForRound(-1);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameControlScript = FindFirstObjectByType<gameControlScript>();
        target = GameObject.Find("Base");
        SetClass();

    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        waterParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        fireParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (HasPathToBase())
        {
            agent.SetDestination(target.transform.position);
        }
        else
        {
            HandleBarricade();
        }
        Die();
    }

    private bool HasPathToBase()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.transform.position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void HandleBarricade()
    {
        barricades = new List<GameObject>(GameObject.FindGameObjectsWithTag("Barricade"));
        if (currentTargetBarricade == null || !barricades.Contains(currentTargetBarricade))
        {
            currentTargetBarricade = FindClosestBarricade();
        }

        if (currentTargetBarricade != null)
        {
            agent.SetDestination(currentTargetBarricade.transform.position);

            if (Vector3.Distance(transform.position, currentTargetBarricade.transform.position) < 1.5f)
            {
                Destroy(currentTargetBarricade);
                barricades.Remove(currentTargetBarricade);
                currentTargetBarricade = null;
            }
        }
    }

    private GameObject FindClosestBarricade()
    {
        GameObject closest = null;
        float shortestDistance = float.MaxValue;

        foreach (var barricade in barricades)
        {
            float distance = Vector3.Distance(transform.position, barricade.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = barricade;
            }
        }

        return closest;
    }
    public void OnDestroyed()
    {
        
    }
    public void Die()
    {
        if (hp <= 0)
        {
            gameControlScript.ChangeGold(reward);
            gameControlScript.CheckForRound(-1);
            Destroy(gameObject);
        }
    }

    public void ApplySlow()
    {
        if (isSlowed)
        {
            StopCoroutine(slowCoroutine);
            agent.speed = ms;
            slowCoroutine = StartCoroutine(SlowEffect(3f));
        }
        else
        {
            slowCoroutine = StartCoroutine(SlowEffect(3f));
        }
    }

    private IEnumerator SlowEffect(float duration)
    {
        float originalSpeed = agent.speed;
        agent.speed = agent.speed * 0.66f;
        isSlowed = true;
        waterParticles.Play();
        yield return new WaitForSeconds(duration);
        agent.speed = originalSpeed;
        isSlowed = false;
    }

    public void ApplyBurn()
    {
        if (isBurning)
        {
            StopCoroutine(burnCoroutine);
            agent.speed = ms;
            burnCoroutine = StartCoroutine(BurnEffect(3f));
        }
        else
        {
            burnCoroutine = StartCoroutine(BurnEffect(3f));
        }
    }

    private IEnumerator BurnEffect(float duration)
    {
        isBurning = true;
        fireParticles.Play();
        int i = 6;
        while (i > 0)
        {
            yield return new WaitForSeconds(duration / 6);
            hp = hp - 10;
            i = i - 1;
        }
        isBurning = false;
    }

    private void SetClass()
    {
        switch (enemyClass)
        {
            case EnemyClass.Light:
                hp = 100;
                agent.speed = 2f;
                ms = agent.speed;
                reward = UnityEngine.Random.Range(3, 9);
                damage = 1;
                break;
            case EnemyClass.Heavy:
                hp = 400;
                agent.speed = 1f;
                ms = agent.speed;
                reward = UnityEngine.Random.Range(8, 14);
                damage = 3;
                break;
        }
    }

    public void OnEnemyDeath()
    {
    }
}