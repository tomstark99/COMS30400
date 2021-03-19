using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class GuardAIPhoton : MonoBehaviourPun
{
    public enum State
    {
        Patroling,
        Alerted,
        Chasing
    }

    public NavMeshAgent guard;
    public LayerMask groundMask, playerMask, obstacleMask;
    public Transform[] points;
    public Light spotlight;
    public State guardState;
    public float timeAlerted;
    public float timeChasing;
    public Vector3 alertPosition;
    public GameObject[] players;


    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public float proximityRange;
    public bool playerSpotted;
    public float guardAngle;
    public Color spotlightColour;
    public Color alertColour;

    void Start()
    {
        // find players, set the guard to its own navmeshagent and set the guard state to patroling
        players = GameObject.FindGameObjectsWithTag("Player");
        guard = GetComponent<NavMeshAgent>();
        guardState = State.Patroling;
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
    
    // finds closest player to the guard so they chase that player
    Transform FindClosestPlayer()
    {
        Transform closestPlayer = null;

        float lastDistance = float.MaxValue;
        // loops through players
        foreach (var player in players)
        {
            Debug.Log(player);
            // finds distance between player and guard
            float eDistance = Vector3.Distance(player.transform.position, transform.position);

            // changes current closestPlayer if none set or if old distance further than new distance
            if (closestPlayer == null || eDistance < lastDistance)
            {
                closestPlayer = player.transform;
                lastDistance = eDistance;
            }
        }

        return closestPlayer;
    }

    // guard chases player
    void ChasePlayer()
    {
        // gets closest player
        Transform closestPlayer = FindClosestPlayer();

        // sets the guard destination to player's position
        transform.LookAt(closestPlayer.transform);
        guard.SetDestination(closestPlayer.position - new Vector3(proximityRange, 0, 0));
        // sets the guard's alert position to the player's current position (so when the player goes out of range, the guard will run to the last place they saw the player)
        guard.gameObject.GetComponent<GuardAIPhoton>().alertPosition = closestPlayer.position;
    }

    bool PlayerSpotted()
    {
        // checks if player is within proximity (so if player gets too close behind guard he will also be spotted)
        if (Physics.CheckSphere(transform.position, proximityRange, playerMask))
        {
            return true;
        }

        foreach (var player in players)
        {
            //Debug.Log(player);
            if (Vector3.Distance(transform.position, player.transform.position) < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                if (guardPlayerAngle < guardAngle / 2f)
                {
                    // checks if guard line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the second child (cube) so it checks if it can see his feet and the bottom of the cube
                    if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask) || !Physics.Linecast(transform.position, player.transform.GetChild(2).transform.position, obstacleMask))
                    {
                        Debug.Log(player.transform.position);
                        Debug.Log(player.transform.GetChild(2).transform.position);
                        return true;
                    }
                }

            }
            
            // checks if player is in guard's view range 
        }
        return false;
    }

    void ChangeToYellow()
    {
        spotlight.color = spotlightColour;
    }

    void ChangeToRed()
    {
        spotlight.color = Color.red;
    }

    // sets all guards in the same 'group' to be alerted to the player
    void SetGuardsToAlerted()
    {
        Transform parent = transform.parent;
        Transform closestPlayer = FindClosestPlayer();
        foreach (Transform child in parent)
        {
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            // sets the guard to be alerted if they're not chasing
            if (temp.guardState != State.Chasing)
            {
                temp.guardState = State.Alerted;
            }
            // resets time alerted
            temp.timeAlerted = 0f;
            // sets alerted position to be the closest player's position
            temp.alertPosition = closestPlayer.position;
        }

    }
    // if item falls next to guard, alert all guards to go to where it fell
    void SetGuardsToAlertedItem(Vector3 position)
    {
        Transform parent = transform.parent;
        foreach (Transform child in parent)
        {
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            temp.guardState = State.Alerted;
            temp.timeAlerted = 0f;
            temp.alertPosition = position;
        }

    }

    void ChangeToOrange()
    {
        spotlight.color = alertColour;
    }

    void GoToSighting()
    {
        guard.SetDestination(alertPosition);
    }
    // guard checks if a rock dropped next to them
    Vector3 CheckForRock()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");
        
        foreach (GameObject rock in rocks)
        {
            //Debug.Log(rock);
            //Debug.Log("It finds rocks");

            // gets the rock alert component
            RockHitGroundAlert tempRock = rock.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RockHitGroundAlert>();

            // if the rock has hit the ground check whether the distance is close enough for the guard to alert other guards
            if (tempRock.rockHitGround)
            {
                Debug.Log(Vector3.Distance(transform.position, tempRock.transform.position));
                if (Vector3.Distance(transform.position, tempRock.transform.position) < 30)
                {
                    return tempRock.transform.position;
                }
            }
        }
        return new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        // Check if player is in guard's sight
        playerSpotted = PlayerSpotted();

        Vector3 rockPos = CheckForRock();

        //if (timeChasing > 8f)
        //{
        //    Debug.Log("You lose");
        //}
        // If the player is not spotted but the guard is in the alerted state
        if (!playerSpotted && guardState == State.Alerted)
        {
            // increase time and once it hits limit, go back to patroling
            timeAlerted += Time.deltaTime;
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
        else if (!playerSpotted && guardState == State.Chasing)
        {
            timeAlerted = 0;
            timeChasing = 0;
            guardState = State.Alerted;
        }
        else if (rockPos != new Vector3(0f, 0f, 0f))
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
