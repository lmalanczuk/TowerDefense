using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//ustawienie celu jako baza


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private GameObject target;
    private NavMeshAgent agent;
    private int hp = 100;

    private RaycastHit[] hits = new RaycastHit[1];

    private void Start()
    {
        target = GameObject.Find("Base");
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        agent.SetDestination(target.transform.position);
        if(hp<=0)
        {
            Destroy(gameObject);
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("kolizja");
            hp=hp-50;
            Destroy(collision.gameObject);
        }
    }
}
