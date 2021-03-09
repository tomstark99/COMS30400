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

    Transform FindClosestPlayer()
    {
        Transform closestPlayer = null;

        float lastDistance = float.MaxValue;

        foreach (var player in players)
        {
            Debug.Log(player);
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
        guard.SetDestination(closestPlayer.position - new Vector3(proximityRange, 0, 0));
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

    void SetGuardsToAlerted()
    {
        Transform parent = transform.parent;
        Transform closestPlayer = FindClosestPlayer();
        foreach (Transform child in parent)
        {
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            if (temp.guardState != State.Chasing)
            {
                temp.guardState = State.Alerted;
            }
            temp.timeAlerted = 0f;
            temp.alertPosition = closestPlayer.position;
        }

    }

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

    Vector3 CheckForRock()
    {
        /*GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {
            RockHitGroundAlert tempRock = rock.transform.GetChild(0).gameObject.GetComponent<RockHitGroundAlert>();
            if (tempRock.rockHitGround)
            {
                Debug.Log(Vector3.Distance(transform.position, tempRock.transform.position));
                if (Vector3.Distance(transform.position, tempRock.transform.position) < 30)
                {
                    return tempRock.transform.position;
                }
            }
        }*/
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
