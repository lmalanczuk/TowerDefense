using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//ustawienie celu jako baza


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScript : MonoBehaviour
{
    private GameObject target;

    private NavMeshAgent agent;

    private RaycastHit[] hits = new RaycastHit[1];


    private int hp; 
    public int Health
    {
        get { return hp; }
        set { hp = value; }
    }

    private float ms;
    private int reward;

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

   

    private void Start()
    {
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
        
        agent.SetDestination(target.transform.position);
        Die();
        
    }
    public void Die()
    {
        if (hp <= 0)
        {
            FindFirstObjectByType<gameControlScript>().ChangeGold(reward);
            Destroy(gameObject);
        }
    }
    
    public void ApplySlow()
    {
        if (isSlowed)
        {
            StopCoroutine(slowCoroutine);
            agent.speed = ms;
            slowCoroutine=StartCoroutine(SlowEffect(3f));
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
            yield return new WaitForSeconds(duration/6);
            hp = hp - 10;
            i =i - 1;
        }
        isBurning=false;
    }

    private void SetClass()
    {
        switch (enemyClass)
        {
            case EnemyClass.Light:
                hp = 100;
                agent.speed = 2f;
                ms = agent.speed;
                reward = UnityEngine.Random.Range(7, 13);
                break;
            case EnemyClass.Heavy:
                hp= 200;
                agent.speed = 1f;
                ms = agent.speed;
                reward = UnityEngine.Random.Range(17, 23); ;
                break;
        }
    }

    
}
