using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerMovementPhoton : MonoBehaviourPun
{
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    public GameObject faceUI;
    public GameObject LeftHandUpUI;
    public GameObject RightHandUpUI;
    private float gravity = -17f;
    private float speedWalking = 4.0f;
    private float speedRunning = 8.0f;
    private float speed = 4.0f;
    private float jumpHeight = 1.5f;

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private Vector3 velocity;
    private Vector3 ladderCentreLine;
    private GameObject train;
    private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool climbing;
    private bool climbingBuilding;
    private bool crouching;
    [SerializeField]
    private bool onTrain;
    private bool check = false;
    private Transform prev;
    private PhotonView PV;
    private AudioSource steps;
    private AudioSource run;

    private bool gameEnding;

    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [SerializeField]
    private GameObject caughtByGuardsText;

    private bool babySteps;

    public bool onMenu;

    // How long the player needs to stay at location
    public float timerCountDown = 0.2f;
    public bool OnTrain
    {
        get { return onTrain; }
    }

    void Start()
    {
        // activates player's camera if its theirs and disables all others
        if (photonView.IsMine)
        {
            // transform.Find("Camera").gameObject.SetActive(true);
            transform.Find("Camera/Camera").gameObject.SetActive(true);
            transform.Find("MinimapCamera").gameObject.SetActive(true);

        }

        PV = GetComponent<PhotonView>();
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            //Debug.Log(" DISABLE CONTROLER ");
            GetComponent<PlayerMovementPhoton>().enabled = false;
        }

        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        foreach (var guard in guards)
        {
            guard.GetComponent<GuardAIPhoton>().PlayerCaught += DisablePlayer;
        }

        onMenu = false;
        steps = groundCheck.GetChild(0).GetComponent<AudioSource>();
        run = groundCheck.GetChild(1).GetComponent<AudioSource>();

        // achievement checker (deleting for testing purposes)
        //PlayerPrefs.DeleteKey("BabySteps");

        if (PlayerPrefs.HasKey("BabySteps1"))
            babySteps = true;
        else
            babySteps = false;
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        // movement function
        if (photonView.IsMine) {
            Movement();
        }
    }

    public void EnableVirtualCamera()
    {
        photonView.RPC(nameof(EnableVirtualCameraRPC), RpcTarget.All);
    }

    [PunRPC]
    void EnableVirtualCameraRPC()
    {
        vcam.Priority = 100;
    }

    [PunRPC]
    void DisablePlayerRPC()
    {
        caughtByGuardsText.SetActive(true);
        gameObject.GetComponent<PlayerAnimation>().SetAllFalse();
        gameObject.GetComponent<PlayerAnimation>().enabled = false;
        gameObject.GetComponent<MouseLookPhoton>().enabled = false;
        gameObject.GetComponent<PlayerMovementPhoton>().enabled = false;
    }

    void DisablePlayer()
    {
        photonView.RPC(nameof(DisablePlayerRPC), RpcTarget.All);
    }

    IEnumerator SetFaceActive() {
        if (check == false) {
            faceUI.SetActive(true);
        }
        yield return new WaitForSeconds(0);
        if (Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("C") == 0) {
            faceUI.SetActive(false);
            check = true;
        }
    }

    void DistanceToTrain() {
        // float distance
    }

    public void GameEnding()
    {
        gameEnding = true;
        StartCoroutine(SelfDisable());
    }

    IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(1f);
        this.enabled = false;
    }

    void Movement()
    {
        if (onMenu && !onTrain)
        {
            steps.Stop();
            return;
        }

        // Checks if the groundCheck object is within distance to the ground layer
        bool old = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (old != isGrounded) GetComponent<PlayerAnimation>().setGrounded(isGrounded);

        // Prevents downward velocity from decreasing infinitely if player is on the ground
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        if (gameEnding)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float l = Input.GetAxis("Ladder");

        if (!babySteps) 
        { 
            if (x > 0f || z > 0f)
            {
                GetComponent<Achievements>()?.BabyStepsCompleted();
                babySteps = true;
            }
        }   

        // instantiates move with null so that it can be set depending on climb
        Vector3 move;

        if ((climbingBuilding || climbing) && PoseParser.GETGestureAsString().CompareTo("L") == 0)
        {
            move = transform.up * 0.4f;
        } else if ((climbingBuilding || climbing) && l > 0f)
        {
            move = transform.up * l;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("F") == 0) {
            move = transform.forward; 
        }
        else if (PoseParser.GETGestureAsString().CompareTo("I") == 0) {
            move = transform.right + transform.forward;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("O") == 0) {
            move = transform.right * -1 + transform.forward;
        }
        else
        {
            move = transform.right * x + transform.forward * z;
        }

        //Sticks player to centreline of ladder
        if (climbing)
        {
            //Debug.Log(timerCountDown);
            timerCountDown -= Time.deltaTime;
            if (timerCountDown < 0)
            {
                timerCountDown = 0;
            }
            //faceUI.SetActive(false);
            //Debug.Log(train);
            //Debug.Log(ladderCentreLine);
            /*Vector3 ladderPos = train.transform.position + (train.transform.rotation * ladderCentreLine);
            ladderPos.y = transform.position.y;
            move += ladderPos - transform.position;*/
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * 2 * Time.deltaTime);
        } else if (climbingBuilding)
        {
            timerCountDown -= Time.deltaTime;
            if (timerCountDown < 0)
            {
                timerCountDown = 0;
            }
             transform.Translate(Vector3.up * Input.GetAxis("Vertical") * 2 * Time.deltaTime);
        }
        // if forwards velocity is greater than 0 and inside the climbing collider, add to the vertical height instead of the forward height
        // else if (climbingBuilding && z > 0f)
        // {
        //     move += transform.right * x + transform.up * z;
        // }
        // else
        // {
        //     move = transform.right * x + transform.forward * z;
        // }

        // sticks the player onto the train
        //if (onTrain)
        //{
        //    //StartCoroutine(SetFaceActive());
        //    Vector3 trainMove = Vector3.MoveTowards(gameObject.transform.position, train.transform.position, Time.deltaTime) - train.transform.position;
        //    trainMove.x = -trainMove.x;
        //    trainMove.y = 0f;
        //    trainMove.z = -trainMove.z;
        //    move += trainMove;
        //}

        //Debug.Log(speed);

        controller.Move(move * speed * Time.deltaTime);

        // Checks if jump button is pressed and allows user to jump if they are on the ground
        if (Input.GetButtonDown("Jump") && isGrounded && !crouching)
        {
            onTrain = false;
            faceUI.SetActive(false);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if ((Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("C") == 0) && !crouching)
        {
            crouching = true;
            faceUI.SetActive(false);
            controller.height = 1.2f;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("N") == 0) && crouching)
        {
            crouching = false;
            controller.height = 1.8f;
        }

        //velocity.y += gravity * Time.deltaTime;
        // Only applies gravity when not climbing, this allows players to stay on ladder
        if (!climbing && !climbingBuilding)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        if ((move.x != 0 || move.z != 0) && isGrounded && !onTrain)
        {
            if (crouching)
            {
                steps.Stop();
                run.Stop();
                return;
            }
                
            if (!steps.isPlaying && speed == 4.0f)
            {
                steps.Play();
                run.Stop();
            }  
            else if (!run.isPlaying && speed == 8.0f)
            {
                run.Play();
                steps.Stop();
            }
        }
        else
        {
            steps.Stop();
            run.Stop();
        }

    }

    [PunRPC]
    void ChangeOnTrainToTrue()
    {
        onTrain = true;
    }

    [PunRPC]
    void ChangeOnTrainToFalse()
    {
        onTrain = false;
    }

    // trigger collider for the ladder and the train floor
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "locomotive")
        {
            timerCountDown = 0.2f;
            // Debug.Log("PLAYER ENTERED LADDER");
            train = other.gameObject;
            ladderCentreLine = ((BoxCollider) other).center;
            // Debug.Log("LADDER COORDS" + (train.transform.position + ladderCentreLine).ToString());
            climbing = true;
            GetComponent<PlayerAnimation>().setClimbing(true);
            
        }
        else if (other.gameObject.tag == "ladder")
        {
            timerCountDown = 0.2f;
            // Debug.Log("ENTER LADDER");
            climbingBuilding = true;
            ladderCentreLine = other.gameObject.transform.position + (other.gameObject.transform.rotation * ((BoxCollider) other).center);
           
            GetComponent<PlayerAnimation>().setClimbing(true);
        } 
        else if (other.gameObject.tag == "trainfloor")
        {
            //Debug.Log("stef is aiiiir");
            train = other.gameObject;
            climbing = false;
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
            GetComponent<PlayerAnimation>().setClimbing(false);
           // photonView.RPC(nameof(ChangeOnTrainToTrue), RpcTarget.All);

            onTrain = true;
            GetComponent<PlayerOnTrain>().OnTrain = true;
            GetComponent<Achievements>().ChooChooCompleted();
        } 
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "locomotive")
        {
             //Debug.Log("PLAYER is on the ladder");
            if(timerCountDown <= 0)
            {
               LeftHandUpUI.SetActive(true);
               RightHandUpUI.SetActive(true);
            }
            
        }
        else if (other.gameObject.tag == "ladder")
        {
            // Debug.Log("ENTER LADDER");
           if(timerCountDown <= 0)
            {
               LeftHandUpUI.SetActive(true);
               RightHandUpUI.SetActive(true);
            }
        } 
    }
    void OnTriggerExit(Collider other)
    {
        if (climbing && other.gameObject.tag == "locomotive")
        {
            timerCountDown = 0.2f;
            // Debug.Log("player stopped climbing");
            climbing = false;
            GetComponent<PlayerAnimation>().setClimbing(false);
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
        }
        else if (climbingBuilding && other.gameObject.tag == "ladder")
        {
            timerCountDown = 0.2f;
            // Debug.Log("EXIT LADDER");
            climbingBuilding = false;
            GetComponent<PlayerAnimation>().setClimbing(false);
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
        }
        if (other.gameObject.tag == "trainfloor")
        {
            //train = null;
            //photonView.RPC(nameof(ChangeOnTrainToFalse), RpcTarget.All);
            onTrain = false;
            GetComponent<PlayerOnTrain>().OnTrain = false;
            //Debug.Log("stef is NOT aiiiir");
        }
    }

    public bool getGrounded() {
        return this.isGrounded;
    }

    public float getSpeedWalking() {
        return this.speedWalking;
    }
    
    public float getSpeedRunning() {
        return this.speedRunning;
    }

    public float getSpeed() {
        return this.speed;
    }

    public void setSpeedRunning(float val) {
        this.speedRunning = val;
    }

    public void setSpeedWalking(float val) {
        this.speedWalking = val;
    }

    public void setSpeed(float val) {
        if(this.speed != val) {
            this.speed = val;
        }
    }

    public void setGrounded(bool val) {
        this.isGrounded = val;
    }
}
