using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{

    //Variables For Enemy Chasing
    [Header("Enemy Chase Destination & Speed")]
    public GameObject target;

    public float chaseSpeed;




    //NavMesh Chase Player
    public void Update()
    {
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        nav.speed = chaseSpeed;
        nav.destination = target.transform.position;
    }
}
