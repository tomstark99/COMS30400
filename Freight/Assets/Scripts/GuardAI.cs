using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class GuardAI : NetworkBehaviour
{
    public NavMeshAgent guard;
    public LayerMask groundMask, playerMask, obstacleMask;
    public Transform[] points;
    public Light spotlight;
    private NetworkManagerMain room;
    private List<Player> players;


    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public float proximityRange;
    public bool playerSpotted;
    public float guardAngle;
    public Color spotlightColour;

    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }

    private void Start()
    {
        guard = GetComponent<NavMeshAgent>();
        players = Room.GamePlayers;
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
        Transform closestPlayer = null;

        float lastDistance = float.MaxValue;

        foreach (Player player in players)
        {
            float eDistance = Vector3.Distance(player.transform.position, transform.position);

            if (closestPlayer == null || eDistance < lastDistance)
            {
                closestPlayer = player.transform;
                lastDistance = eDistance;
            }
        }

        transform.LookAt(closestPlayer.transform);
        guard.SetDestination(closestPlayer.position- new Vector3(proximityRange,0,0));
    }

    bool PlayerSpotted()
    {
        // checks if player is within proximity (so if player gets too close behind guard he will also be spotted)
        if (Physics.CheckSphere(transform.position, proximityRange, playerMask))
        {
            return true;
        }

        foreach (Player player in players)
        {
            // checks if player is in guard's view range 
            if (Vector3.Distance(transform.position, player.transform.position) < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                if (guardPlayerAngle < guardAngle / 2f)
                {
                    // checks if guard line of sight is blocked by an obstacle
                    if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask))
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    [ClientRpc]
    void changeToYellow()
    {
        spotlight.color = spotlightColour;
    }

    [ClientRpc]
    void changeToRed()
    {
        spotlight.color = Color.red;
    }

    [Server]
    // Update is called once per frame
    void Update()
    {
        if (isServer == false)
        {
            return;
        }

        // Check if player is in guard's sight
        playerSpotted = PlayerSpotted();

        // If the player is not spotted and the guard has reached their destination, go to new point
        if (!playerSpotted && guard.remainingDistance < 0.5f)
        {
            GotoNextPoint();
            changeToYellow();
        }
        if (playerSpotted)
        {
            ChasePlayer();
            changeToRed();
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * sightRange);
    }
}
