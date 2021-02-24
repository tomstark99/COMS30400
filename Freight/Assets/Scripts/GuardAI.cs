using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class GuardAI : NetworkBehaviour
{
    public enum State
    {
        Chasing,
        Patroling,
        Alerted
    }

    public NavMeshAgent guard;
    public LayerMask groundMask, playerMask, obstacleMask;
    public Transform[] points;
    public Light spotlight;
    private NetworkManagerMain room;
    private List<Player> players;
    public State guardState;
    public float timeAlerted;
    public float timeChasing;
    public Vector3 alertPosition;


    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public float proximityRange;
    public bool playerSpotted;
    public float guardAngle;
    public Color spotlightColour;
    public Color alertColour;

    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }

    [ServerCallback]
    void Start()
    {
        players = Room.GamePlayers;
        guard = GetComponent<NavMeshAgent>();
        guardState = State.Patroling;
    }

    //public override void OnStartServer()
    //{
    //    Debug.Log("this ran");
    //    players = Room.GamePlayers;
    //    guard = GetComponent<NavMeshAgent>();
    //    guardState = State.Patroling;
    //}

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

    Transform FindClosestPlayer()
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

        return closestPlayer;
    }

    void ChasePlayer()
    {
        Transform closestPlayer = FindClosestPlayer();

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
    void ChangeToYellow()
    {
        spotlight.color = spotlightColour;
    }

    [ClientRpc]
    void ChangeToRed()
    {
        spotlight.color = Color.red;
    }

    void SetGuardsToAlerted()
    {
        Transform parent = transform.parent;
        Transform closestPlayer = FindClosestPlayer();
        foreach (Transform child in parent)
        {
            GuardAI temp = child.gameObject.GetComponent<GuardAI>();
            temp.guardState = State.Alerted;
            temp.timeAlerted = 0f;
            temp.alertPosition = closestPlayer.position;
        }

    }

    void SetGuardsToAlertedItem(Vector3 position)
    {
        Transform parent = transform.parent;
        foreach (Transform child in parent)
        {
            GuardAI temp = child.gameObject.GetComponent<GuardAI>();
            temp.guardState = State.Alerted;
            temp.timeAlerted = 0f;
            temp.alertPosition = position;
        }

    }

    [ClientRpc]
    void ChangeToOrange()
    {
        spotlight.color = alertColour;
    }

    void GoToSighting()
    {
        guard.SetDestination(alertPosition);
    }

    Vector3 CheckForRock()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {
            RockHitGroundAlert tempRock = rock.transform.GetChild(0).gameObject.GetComponent<RockHitGroundAlert>();
            if (tempRock.rockHitGround)
            {
                Debug.Log(Vector3.Distance(transform.position, tempRock.transform.position));
                if (Vector3.Distance(transform.position, tempRock.transform.position) < 100)
                {
                    return tempRock.transform.position;
                }
            }
        }
        return new Vector3 (0f,0f,0f);
    }

    [ServerCallback]
    // Update is called once per frame
    void Update()
    {
        if (isServer == false)
        {
            return;
        }

        players = Room.GamePlayers;

        // Check if player is in guard's sight
        playerSpotted = PlayerSpotted();

        Vector3 rockPos = CheckForRock();

        // If the player is not spotted but the guard is in the alerted state
        if (!playerSpotted && guardState == State.Alerted)
        {
            // increase time and once it hits limit, go back to patroling
            timeAlerted += Time.deltaTime;
            Debug.Log(timeAlerted);
            if (timeAlerted > 8f)
            {
                guardState = State.Patroling;
                GotoNextPoint();
                ChangeToYellow();
                timeAlerted = 0f;
            }
            else
            {
                GoToSighting();
                ChangeToOrange();
            }
        }
        else if (rockPos != new Vector3 (0f,0f,0f))
        {
            SetGuardsToAlertedItem(rockPos);
        }
        // If the player is not spotted and the guard has reached their destination, go to new point
        else if (!playerSpotted && guard.remainingDistance < 0.5f)
        {
            guardState = State.Patroling;
            timeChasing = 0f;
            GotoNextPoint();
            ChangeToYellow();
        }
        // If player is spotted, guard will chase player and set guards in the same group as him to spotted
        else if (playerSpotted)
        {
            timeChasing += Time.deltaTime;
            if (timeChasing > 2f)
            {
                SetGuardsToAlerted();
            }
            guardState = State.Chasing;
            ChasePlayer();
            ChangeToRed();
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * sightRange);
    }
}
