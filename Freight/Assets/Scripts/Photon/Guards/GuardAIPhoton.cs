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
    // states that decide how the guard will behave
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

        // checks if the map has EndGame scripts and then subscribes to the correct events depending on whether the map is the first or second one
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

        // subscribe to events on the spotlights that will set the guards to be alerted when player steps in the spotlights
        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpinningLight");
        foreach (var light in lights)
        {
            light.GetComponent<RotateLight>().PlayerInLight += SetAllGuardsToAlerted;
        }

        // checks game settings and sets the different guard settings
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

        // subscribe to event that lets other guards know when the player has been caught 
        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += PlayerHasBeenCaught;
        }

        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        // subscribe to event that lets guard know when a rock has hit the ground
        foreach (GameObject rock in rocks)
        {
            // gets the rock alert component
            rock.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RockHitGroundAlert>().RockHitGround += CheckForRock;

        }

        playerCaught = false;

        // gets walking and running sounds of the guards as well as chase and normal music so guards can swap them around depending on if they're patroling or chasisng
        walk = sounds.transform.GetChild(0).GetComponent<AudioSource>();
        run = sounds.transform.GetChild(1).GetComponent<AudioSource>();
        chaseMusic = globalSounds.transform.GetChild(1).GetComponent<AudioSource>();
        normalMusic = globalSounds.transform.GetChild(0).GetComponent<AudioSource>();

    }

    // this is called when the guard is disabled which usually happens when they are killed
    // we unsubscribe from all events they were subscribed to so we don't get null reference errors after they have been destroyed
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

    // this is called when a guard is killed to check if any other guards are alerted or chasing, if there are none we change the music back to normal music
    public void CheckMusicOnGuardDeath()
    {
        // set dead guard state to patroling just for the purpose of the CheckIfAllGuardsPatroling function call not returning true if the dead guard was the only one chasing
        guardState = State.Patroling;
        bool changeMusicBack = CheckIfAllGuardsPatroling();
        // if all guards were patroling, we send an RPC to let the players know to change music
        if (changeMusicBack)
        {
            photonView.RPC(nameof(ResetMusic), RpcTarget.All);
        }
    }

    // this function is subscribed to the EndGame class and ran once the game is over
    // this is done so when the player is already on the train or getaway vehicle, the guards cant catch them during the final cutscene and somehow catch them
    public void DisableGuards()
    {
        GetComponent<NavMeshAgent>().gameObject.SetActive(false);
        guardState = State.Patroling;
    }

    // traverses throguh all the points in the array of the guard's patrol path
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

        // makes guard face the player 
        transform.LookAt(closestPlayer.transform);

        // sets the guard destination to player's position
        guard.SetDestination(closestPlayer.position - new Vector3(1f, 0, 0));

        // sets the guard's alert position to the player's current position (so when the player goes out of range, the guard will run to the last place they saw the player)
        alertPosition = closestPlayer.position;
    }

    // checks if the players are in the guard's sight range or if they have been detected by the sound ripples
    bool PlayerSpotted()
    {
        // checks if player is within proximity (so if player gets too close behind guard he will also be spotted)
        if (Physics.CheckSphere(transform.position, proximityRange, playerMask))
        {
            guard.speed = speedChasing;
            return true;
        }

        // loops through all players in the level
        foreach (var player in players)
        {
            // checks distance between guard and player
            var distance = Vector3.Distance(transform.position, player.transform.position);

            // if distance is less than the guard's sight range, we may proceed to the next check
            if (distance < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                // if player is within the angle of the guard's spotlight, we may proceed to the next check
                if (guardPlayerAngle < guardAngle / 2f)
                {
                    // checks if guard line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the player's head so it checks if it can see player's feet and the player's head
                    if (!Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, player.transform.Find("master/Reference/Hips/LeftUpLeg/LeftLeg/LeftFoot").transform.position, obstacleMask) || !Physics.Linecast(transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask))
                    {
                        // checks if player has achieved these achievements
                        player.GetComponent<Achievements>()?.LearnTheHardWayCompleted();
                        player.GetComponent<Achievements>()?.WasDetected();
                        // set guard speed to chasing and return true as player is within the guard's line of sight
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

        // if we get here, this means the players are not detected by the guard and we can set the guard's speed to patroling and return false
        guard.speed = speedPatrolling;
        return false;
    }

    // checks if a dead guard is in the guard's sight range 
    bool DeadGuardSpotted()
    {
        GameObject[] deadGuards = GameObject.FindGameObjectsWithTag("DeadGuard");

        foreach (var deadGuard in deadGuards)
        {
            if (Vector3.Distance(transform.position, deadGuard.transform.position) < sightRange)
            {
                // vector from guard to dead guard
                Vector3 dirToPlayer = (deadGuard.transform.position - transform.position).normalized;
                float guardPlayerAngle = Vector3.Angle(transform.forward, dirToPlayer);
                if (guardPlayerAngle < guardAngle / 2f)
                {
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

    // sets all guards in the level to be alerted
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

    // sets all guards that are a child of the same GameObject to be alerted to the player
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

        // loops through all guards that are children of the same GameObject
        foreach (Transform child in parent)
        {
            // sets the guard to be alerted, resets their time alerted and changes their alert position to where the item had landed
            GuardAIPhoton temp = child.gameObject.GetComponent<GuardAIPhoton>();
            temp.guardState = State.Alerted;
            temp.timeAlerted = 0f;
            temp.alertPosition = position;
        }

    }

    // changes the guard's light to yellow, representing patroling
    [PunRPC]
    void ChangeToYellowRPC()
    {
        spotlight.color = spotlightColour;
    }

    void ChangeToYellow()
    {
        photonView.RPC(nameof(ChangeToYellowRPC), RpcTarget.All);
    }

    // changes the guard's light to red, representing chasing
    [PunRPC]
    void ChangeToRedRPC()
    {
        spotlight.color = Color.red;
    }

    void ChangeToRed()
    {
        photonView.RPC(nameof(ChangeToRedRPC), RpcTarget.All);
    }

    // changes the guard's light to orange, representing being alerted
    [PunRPC]
    void ChangeToOrangeRPC()
    {
        spotlight.color = alertColour;
    }

    void ChangeToOrange()
    {
        photonView.RPC(nameof(ChangeToOrangeRPC), RpcTarget.All);
    }

    // called on an alerted guard, makes them go to the last sighting of the player or to the place an item had landed
    void GoToSighting()
    {
        guard.SetDestination(alertPosition);
    }

    // sets guards destination to the closest player to them, this is called when a dead guard has been spotted and all guards are alerted
    void GoToPlayer()
    {
        Transform closestPlayer = FindClosestPlayer();
        guard.SetDestination(closestPlayer.position);
    }

    // function is subscribed to rock event for when rock hits ground, checks which rock hit the ground
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
    
    // this function is subcribed to an event on all the other guards
    // the purpose of this boolean being set to true is so that when a player has been caught, all guards stop their logic in the Update function so we don't get multiple guards getting
    // player caught logic once one guard has already caught the player
    void PlayerHasBeenCaught()
    {
        playerCaught = true;
    }

    // used to check if all guards are patroling
    // use of this is done to check if music is able to be changed back to normal music from chase music as well as checking if player completed the achievement
    bool CheckIfAllGuardsPatroling()
    {
        GameObject[] allGuards = GameObject.FindGameObjectsWithTag("Guard");
        // loops through guards and checks if they are patroling
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

    // RPC call to all players to reset music back to normal
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

    // RPC call to all players to start chase music and stop normal music
    [PunRPC]
    void StartChaseMusic()
    {
        chaseMusic.Play();
        normalMusic.Stop();
    }
    
    // RPC call to all players to set the guard's patience bar to inactive
    [PunRPC]
    void SetPatienceBarToFalse()
    {
        patienceBar.gameObject.SetActive(false);
    }

    // RPC call to all players to set the guard's patience bar to active
    [PunRPC]
    void SetPatienceBarToTrue()
    {
        patienceBar.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // plays walking sound effect if guard is not chasing
        if (!walk.isPlaying && guardState != State.Chasing)
        {
            walk.Play();
            run.Stop();
        }

        // returns early if not master client so only master client controls guard's actions
        if (!PhotonNetwork.IsMasterClient)
            return;

        // we check if the guard is still calculating a path to its point as NavMeshAgents automatically set the distance to the next point to 0 while they are calculating
        // if they don't calculate the path within a frame, it will be stuck on 0 forever and the guard will be stuck in the same place until they become alerted or chasing
        // this early return is called to prevent this from happening 
        if ((guard.pathPending && guardState == State.Patroling) || playerCaught)
            return;

        players = GameObject.FindGameObjectsWithTag("Player");

        // saves old player spotted state
        bool old = playerSpotted;

        // checks if guard has spotted a player or a dead guard
        playerSpotted = PlayerSpotted();
        deadGuardSpotted = DeadGuardSpotted();

        // if old state different to new state, change chasing state on PlayerAnimation
        if(old != playerSpotted) 
        {
            GetComponent<GuardAnimation>().setChasing(playerSpotted);
        }

        // if chase music is not playing and the guard is in another state other than patroling, send RPC to all players to play chase music
        if (!chaseMusic.isPlaying && guardState != State.Patroling)
        {
            photonView.RPC(nameof(StartChaseMusic), RpcTarget.All);
        }
            
        // if the player has been spotted for over 8 seconds, the game is over and we enable the cinemachine camera of the closest player to show they were the ones caught to the other player
        if (playerSpotted && timeChasing > 8f)
        {
            PlayerCaught();
            Transform closestPlayer = FindClosestPlayer();
            closestPlayer.GetComponent<PlayerMovementPhoton>().EnableVirtualCamera();
            return;
        }

        // if a dead guard has been spotted, we set all guards in the level to be alerted
        if (deadGuardSpotted)
        {
            SetAllGuardsToAlerted();
        }

        // logic for when a player is not spotted but they are alerted due to a dead guard being spotted
        if (!playerSpotted && guardState == State.DeadGuardAlerted)
        {
            timeAlerted += Time.deltaTime;
            // if they have been alerted in this state for over 15 seconds, they can go back to patroling
            if (timeAlerted > 15f)
            {
                guardState = State.Patroling;
                guard.ResetPath();
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
                // if the time is less than 15, go to the nearest player
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
        // if player is not spotted but the most recent guard state was chasing, reset patience bar and set the guard to be alerted
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
            // sets patience bar to true for all players
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
            // if guard chases a player for more than 2 seconds, alert all guards in the same GameObject to be alerted as well
            if (timeChasing > 2f)
            {
                SetGuardsToAlerted();
            }
            guardState = State.Chasing;
            ChasePlayer();
            ChangeToRed();
        }

    }

    // used for debugging in Unity Editor, draws red lines that represent the guard's sight range
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * sightRange);
    }

    public bool getSpotted() {
        return this.playerSpotted;
    }

    // this function is called several times per second, the master client writes to the stream while the other client reads the data the master client wrote several times a second
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // master client sends the guard's state, the time chasing if the patience bar is active and if the player has been spotted
        if (stream.IsWriting)
        {
            stream.SendNext(guardState);
            if (patienceBar.gameObject.activeSelf)
            {
                stream.SendNext(timeChasing);
            }
            stream.SendNext(playerSpotted);
        }
        // other client recieves the guard's state, the time chasing if the patience bar is active and if the player has been spotted
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
