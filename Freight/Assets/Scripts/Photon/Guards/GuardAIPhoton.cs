using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;

// https://docs.unity3d.com/Manual/nav-AgentPatrol.html 
public class GuardAIPhoton : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum State
    {
        Patroling,
        Alerted,
        DeadGuardAlerted,
        Chasing
    }

    [SerializeField]
    private bool reactsToRocks;

    [SerializeField]
    private Slider patienceBar;

    public NavMeshAgent guard;
    public LayerMask groundMask, playerMask, obstacleMask;
    public Transform[] points;
    public Light spotlight;
    public GameObject sounds;
    [SerializeField]
    private State guardState;
    private float timeAlerted;
    private float timeChasing;
    private Vector3 alertPosition;
    private GameObject[] players;

    public float speedPatrolling = 3.0f;
    public float speedChasing = 7.0f;

    // Counter to increment points in path
    private int destPoint = 0;

    public float sightRange;
    public float proximityRange;
    private bool playerSpotted;
    private bool deadGuardSpotted;
    public float guardAngle;
    public Color spotlightColour;
    public Color alertColour;

    private EndGame endGame;
    private EndGameSecond endGame2;

    public event Action PlayerCaught;
    private bool playerCaught;

    private AudioSource walk;
    private AudioSource run;

    [SerializeField]
    private GameObject globalSounds;

    private AudioSource chaseMusic;
    private AudioSource normalMusic;

    public State GuardState
    {
        get { return guardState; }
    }
    
    void Start()
    {
        // find players, set the guard to its own navmeshagent and set the guard state to patroling
        players = GameObject.FindGameObjectsWithTag("Player");
        guard = GetComponent<NavMeshAgent>();
        guardState = State.Patroling;
        if (GameObject.Find("Endgame") != null)
        {
            if (GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>() != null)
            {
                endGame = GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGame>();
                endGame.EndTheGame += DisableGuards;
            }
            else if (GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGameSecond>() != null)
            {
                endGame2 = GameObject.FindGameObjectWithTag("EndGame").GetComponent<EndGameSecond>();
                endGame2.EndTheGameSecond += DisableGuards;
            }
        }

        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties);

        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpinningLight");
        foreach (var light in lights)
        {
            light.GetComponent<RotateLight>().PlayerInLight += SetAllGuardsToAlerted;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties["GuardSightRange"] != null)
        {
            sightRange = (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardSightRange"];
            spotlight.range = sightRange;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["GuardAngle"] != null)
        {
            guardAngle = (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardAngle"];
            spotlight.spotAngle = guardAngle;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["SpeedChasing"] != null)
        {
            speedChasing = (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedChasing"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["SpeedPatrolling"] != null)
        {
            speedPatrolling = (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedPatrolling"];
        }

        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += PlayerHasBeenCaught;
        }

        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {

            // gets the rock alert component
            rock.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RockHitGroundAlert>().RockHitGround += CheckForRock;

        }

        playerCaught = false;
        walk = sounds.transform.GetChild(0).GetComponent<AudioSource>();
        run = sounds.transform.GetChild(1).GetComponent<AudioSource>();
        chaseMusic = globalSounds.transform.GetChild(1).GetComponent<AudioSource>();
        normalMusic = globalSounds.transform.GetChild(0).GetComponent<AudioSource>();


        //achievement checker
    }

    public override void OnDisable()
    {

        if (endGame != null)
            endGame.EndTheGame -= DisableGuards;

        if (endGame2 != null)
            endGame2.EndTheGameSecond -= DisableGuards;

        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpinningLight");
        foreach (var light in lights)
        {
            light.GetComponent<RotateLight>().PlayerInLight -= SetAllGuardsToAlerted;
        }

        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {
            // gets the rock alert component
            rock.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RockHitGroundAlert>().RockHitGround -= CheckForRock;
        }

    }

    public void CheckMusicOnGuardDeath()
    {
        guardState = State.Patroling;
        bool changeMusicBack = CheckIfAllGuardsPatroling();
        if (changeMusicBack)
        {
            photonView.RPC(nameof(ResetMusic), RpcTarget.All);
        }
    }

    public void DisableGuards()
    {
        GetComponent<NavMeshAgent>().gameObject.SetActive(false);
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
            //Debug.Log(player);
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

        guard.SetDestination(closestPlayer.position - new Vector3(1f, 0, 0));
        //if (Vector3.Distance(transform.position, closestPlayer.position) > 0.5f)
        //{
        //    NavMesh.FindClosestEdge(closestPlayer.position, out NavMeshHit hit, NavMesh.AllAreas);
        //    Debug.Log("hit position: " + hit.position);
        //    Debug.Log("player position: " + closestPlayer.position);
        //    guard.SetDestination(hit.position);
        //}

        // sets the guard's alert position to the player's current position (so when the player goes out of range, the guard will run to the last place they saw the player)
        alertPosition = closestPlayer.position;
    }

    bool PlayerSpotted()
    {
        // checks if player is within proximity (so if player gets too close behind guard he will also be spotted)
        if (Physics.CheckSphere(transform.position, proximityRange, playerMask))
        {
            guard.speed = speedChasing;
            return true;
        }

        foreach (var player in players)
        {
            //Debug.Log(player);
            var distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                if (guardPlayerAngle < guardAngle / 2f)
                {
                    // checks if guard line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the second child (cube) so it checks if it can see his feet and the bottom of the cube
                    if (!Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, player.transform.Find("master/Reference/Hips/LeftUpLeg/LeftLeg/LeftFoot").transform.position, obstacleMask) || !Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask))
                    {
                        player.GetComponent<Achievements>()?.LearnTheHardWayCompleted();
                        player.GetComponent<Achievements>()?.WasDetected();
                        guard.speed = speedChasing;
                        return true;
                    }
                }

            }
            var decibelsValue = player.GetComponent<SoundRipples>().decibelsValue;
            if(distance < decibelsValue)
            {
                player.GetComponent<Achievements>()?.LearnTheHardWayCompleted();
                player.GetComponent<Achievements>()?.WasDetected();
                guard.speed = speedChasing;
                return true;
            }
            
            // checks if player is in guard's view range 
        }
        guard.speed = speedPatrolling;
        return false;
    }

    bool DeadGuardSpotted()
    {
        GameObject[] deadGuards = GameObject.FindGameObjectsWithTag("DeadGuard");

        foreach (var deadGuard in deadGuards)
        {
            if (Vector3.Distance(transform.position, deadGuard.transform.position) < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (deadGuard.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                if (guardPlayerAngle < guardAngle / 2f)
                {
                    //Debug.Log(Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, deadGuard.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask));
                    // checks if guard line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the second child (cube) so it checks if it can see his feet and the bottom of the cube
                    if (!Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, deadGuard.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask))
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    public void SetAllGuardsToAlerted()
    {
        GameObject[] allGuards = GameObject.FindGameObjectsWithTag("Guard");
        Transform closestPlayer = FindClosestPlayer();
        foreach (var guard in allGuards)
        {
            Transform child = guard.transform;
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            // sets the guard to be alerted if they're not chasing
            if (temp.guardState != State.Chasing)
            {
                temp.guardState = State.DeadGuardAlerted;
            }
            // resets time alerted
            temp.timeAlerted = 0f;
            // sets alerted position to be the closest player's position
            temp.alertPosition = closestPlayer.position;
        }
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

    [PunRPC]
    void ChangeToYellowRPC()
    {
        spotlight.color = spotlightColour;
    }


    void ChangeToYellow()
    {
        photonView.RPC(nameof(ChangeToYellowRPC), RpcTarget.All);
    }

    [PunRPC]
    void ChangeToRedRPC()
    {
        spotlight.color = Color.red;
    }

    void ChangeToRed()
    {
        photonView.RPC(nameof(ChangeToRedRPC), RpcTarget.All);
    }

    [PunRPC]
    void ChangeToOrangeRPC()
    {
        spotlight.color = alertColour;
    }

    void ChangeToOrange()
    {
        photonView.RPC(nameof(ChangeToOrangeRPC), RpcTarget.All);
    }

    void GoToSighting()
    {
        //NavMesh.FindClosestEdge(alertPosition, out NavMeshHit hit, NavMesh.AllAreas);
        guard.SetDestination(alertPosition);
        //guard.SetDestination(hit.position);
    }

    void GoToPlayer()
    {
        Transform closestPlayer = FindClosestPlayer();
        guard.SetDestination(closestPlayer.position);
    }

    void CheckForRock()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (GameObject rock in rocks)
        {

            // gets the rock alert component
            RockHitGroundAlert tempRock = rock.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RockHitGroundAlert>();

            // if the rock has hit the ground check whether the distance is close enough for the guard to alert other guards
            if (tempRock.rockHitGround)
            {
                //Debug.Log(Vector3.Distance(transform.position, tempRock.transform.position));
                if (Vector3.Distance(transform.position, tempRock.transform.position) < 30)
                {
                    SetGuardsToAlertedItem(tempRock.transform.position);
                    // checks which player threw the rock and completes achievement
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var player in players)
                    {
                        if (player.GetComponent<PhotonView>().Owner == rock.GetComponent<PhotonView>().Controller)
                        {
                            if(tempRock.GetComponent<Unachievable>() == null)
                                player.GetComponent<Achievements>()?.UseNatureCompleted();
                            return;
                        }
                    }
                    
                    return;
                }
            }
        }

    }

    void PlayerHasBeenCaught()
    {
        playerCaught = true;
    }

    bool CheckIfAllGuardsPatroling()
    {
        GameObject[] allGuards = GameObject.FindGameObjectsWithTag("Guard");
        foreach (var guard in allGuards)
        {
            Transform child = guard.transform;
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            // sets the guard to be alerted if they're not chasing
            if (temp.guardState != State.Patroling)
            {
                return false;
            }
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            player.GetComponent<Achievements>()?.OnTheRunCompleted();
        }

        return true;
    }

    [PunRPC]
    void ResetMusic()
    {
        if (chaseMusic == null || normalMusic == null)
            return;

        if (!normalMusic.isPlaying)
        {
            chaseMusic.Stop();
            normalMusic.Play();
        }
    }

    [PunRPC]
    void StartChaseMusic()
    {
        chaseMusic.Play();
        normalMusic.Stop();
    }

    [PunRPC]
    void SetPatienceBarToFalse()
    {
        patienceBar.gameObject.SetActive(false);
    }

    [PunRPC]
    void SetPatienceBarToTrue()
    {
        patienceBar.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        if (!walk.isPlaying && guardState != State.Chasing)
        {
            walk.Play();
            run.Stop();
        }

        if (!PhotonNetwork.IsMasterClient)
            return;

        if ((guard.pathPending && guardState == State.Patroling) || playerCaught)
            return;

        players = GameObject.FindGameObjectsWithTag("Player");
        // Check if player is in guard's sight

        bool old = playerSpotted;

        playerSpotted = PlayerSpotted();
        deadGuardSpotted = DeadGuardSpotted();

        if(old != playerSpotted) 
        {
            GetComponent<GuardAnimation>().setChasing(playerSpotted);
        }


        if (!chaseMusic.isPlaying && guardState != State.Patroling)
        {
            photonView.RPC(nameof(StartChaseMusic), RpcTarget.All);
        }
            

        if (playerSpotted && timeChasing > 8f)
        {
            PlayerCaught();
            Transform closestPlayer = FindClosestPlayer();
            closestPlayer.GetComponent<PlayerMovementPhoton>().EnableVirtualCamera();
            return;
        }

        if (deadGuardSpotted)
        {
            SetAllGuardsToAlerted();
        }

        if (!playerSpotted && guardState == State.DeadGuardAlerted)
        {
            timeAlerted += Time.deltaTime;
            if (timeAlerted > 15f)
            {
                guardState = State.Patroling;
                guard.ResetPath();
                //GotoNextPoint();
                guard.destination = points[destPoint].position;
                ChangeToYellow();
                timeAlerted = 0f;
                bool changeMusicBack = CheckIfAllGuardsPatroling();
                if (changeMusicBack)
                {
                    photonView.RPC(nameof(ResetMusic), RpcTarget.All);
                }
            }
            else
            {
                GoToPlayer();
                ChangeToOrange();
            }
        }
        // If the player is not spotted but the guard is in the alerted state
        else if (!playerSpotted && guardState == State.Alerted)
        {
            // increase time and once it hits limit, go back to patroling
            timeAlerted += Time.deltaTime;
            if (timeAlerted > 8f)
            {
                guardState = State.Patroling;
                guard.ResetPath();
                //GotoNextPoint();
                guard.destination = points[destPoint].position;
                ChangeToYellow();
                timeAlerted = 0f;
                bool changeMusicBack = CheckIfAllGuardsPatroling();
                if (changeMusicBack)
                {
                    photonView.RPC(nameof(ResetMusic), RpcTarget.All);
                }
            }
            else
            {
                GoToSighting();
                ChangeToOrange();
            }
        }
        else if (!playerSpotted && guardState == State.Chasing)
        {
            patienceBar.value = 0;
            photonView.RPC(nameof(SetPatienceBarToFalse), RpcTarget.All);
            timeAlerted = 0;
            timeChasing = 0;
            guardState = State.Alerted;
        }

        // If the player is not spotted and the guard has reached their destination, go to new point
        else if (!playerSpotted && guard.remainingDistance < 1.0f)
        {
            guardState = State.Patroling;
            timeChasing = 0f;
            GotoNextPoint();
            ChangeToYellow();
        }
        // If player is spotted, guard will chase player and set guards in the same group as him to spotted
        else if (playerSpotted)
        {
            if (patienceBar.gameObject.activeSelf == false)
            {
                photonView.RPC(nameof(SetPatienceBarToTrue), RpcTarget.All);
            }
            // sound
            if (guard.velocity != Vector3.zero)
            {
                if (!run.isPlaying)
                {
                    run.Play();
                    walk.Stop();
                }
            }
            else
            {
                run.Stop();
            }
            timeChasing += Time.deltaTime;
            patienceBar.value = timeChasing;
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

    public bool getSpotted() {
        return this.playerSpotted;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(guardState);
            if (patienceBar.gameObject.activeSelf)
            {
                stream.SendNext(timeChasing);
            }
            stream.SendNext(playerSpotted);
        }
        else if (stream.IsReading)
        {
            guardState = (State) stream.ReceiveNext();
            if (patienceBar.gameObject.activeSelf)
            {
                patienceBar.value = (float) stream.ReceiveNext();
            }
            playerSpotted = (bool) stream.ReceiveNext();
        }
    }
}
