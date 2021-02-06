using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class NewBehaviourScript : MonoBehaviour
{
    public NavMeshAgent guard;
    public Transform player;
    public LayerMask groundMask, playerMask;
    public Transform[] points;

    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public bool playerSpotted;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        guard = GetComponent<NavMeshAgent>();
    }

    void GotoNextPoint()
    {
        // If no points in array, just return
        if (points.Length == 0)
        {
            return;
        }

        // Sets guard's new destination 
        guard.destination = points[destPoint].position;

        // Get new point, modulo so you cycle back to 0
        destPoint = (destPoint + 1) % points.Length;
    }

    void ChasePlayer()
    {
        guard.SetDestination(player.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in guard's sight
        playerSpotted = Physics.CheckSphere(transform.position, sightRange, playerMask);

        // If the player is not spotted and the guard has reached their destination, go to new point
        if (!playerSpotted && guard.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
        if (playerSpotted)
        {
            ChasePlayer();
        }

    }
}
