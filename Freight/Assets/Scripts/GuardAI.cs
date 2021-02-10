using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class GuardAI : MonoBehaviour
{
    public NavMeshAgent guard;
    public Transform player;
    public LayerMask groundMask, playerMask, obstacleMask;
    public Transform[] points;
    public Light spotlight;

    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public float proximityRange;
    public bool playerSpotted;
    float guardAngle;
    Color spotlightColour;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        guard = GetComponent<NavMeshAgent>();
        spotlightColour = spotlight.color;
        guardAngle = spotlight.spotAngle;
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
        transform.LookAt(player.transform);
        guard.SetDestination(player.position- new Vector3(proximityRange,0,0));
    }

    bool PlayerSpotted()
    {
        // checks if player is within proximity (so if player gets too close behind guard he will also be spotted)
        if (Physics.CheckSphere(transform.position, proximityRange, playerMask))
        {
            return true;
        }

        // checks if player is in guard's view range 
        if (Vector3.Distance(transform.position, player.position) < sightRange)
        {
            // vector from guard to player
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
            if (guardPlayerAngle < guardAngle / 2f)
            {
                // checks if guard line of sight is blocked by an obstacle
                if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                {
                    return true;
                }
            }
            
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is in guard's sight
        //playerSpotted = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerSpotted = PlayerSpotted();

        // If the player is not spotted and the guard has reached their destination, go to new point
        if (!playerSpotted && guard.remainingDistance < 0.5f)
        {
            GotoNextPoint();
            spotlight.color = spotlightColour;
        }
        if (playerSpotted)
        {
            ChasePlayer();
            spotlight.color = Color.red;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * sightRange);
    }
}
