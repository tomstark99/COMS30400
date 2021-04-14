using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    private bool onTrain;
    private bool check = false;
    private Transform prev;
    private PhotonView PV;

    public bool onMenu;
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
        }

        PV = GetComponent<PhotonView>();
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            Debug.Log(" DISABLE CONTROLER ");
            GetComponent<PlayerMovementPhoton>().enabled = false;
        }

        onMenu = false;
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

    IEnumerator SetFaceActive() {
        if (check == false) {
            faceUI.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        if (Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("C") == 0) {
            faceUI.SetActive(false);
            check = true;
        }
    }

    void DistanceToTrain() {
        // float distance
    }

    void Movement()
    {
        if (onMenu && !onTrain)
            return;
        // Checks if the groundCheck object is within distance to the ground layer
        bool old = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (old != isGrounded) GetComponent<PlayerAnimation>().setGrounded(isGrounded);

        // Prevents downward velocity from decreasing infinitely if player is on the ground
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float l = Input.GetAxis("Ladder");

        // instantiates move with null so that it can be set depending on climb
        Vector3 move;

        if (climbing && PoseParser.GETGestureAsString().CompareTo("L") == 0)
        {
            move = transform.up * 0.2f;
        } else if (climbing && l > 0f)
        {
            move = transform.up * l;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("F") == 0) {
            move = transform.forward * speed / 3;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("I") == 0) {
            move = transform.right * speed / 4 + transform.forward * speed / 4;
        }
        else if (PoseParser.GETGestureAsString().CompareTo("O") == 0) {
            move = transform.right * (-speed / 4) + transform.forward * speed / 4;
        }
        else
        {
            move = transform.right * x + transform.forward * z;
        }

        //Sticks player to centreline of ladder
        if (climbing)
        {
            //faceUI.SetActive(false);
            Vector3 ladderPos = train.transform.position + (train.transform.rotation * ladderCentreLine);
            ladderPos.y = transform.position.y;
            move += ladderPos - transform.position;
        }
        // if forwards velocity is greater than 0 and inside the climbing collider, add to the vertical height instead of the forward height
        else if (climbingBuilding && z > 0f)
        {
            move = transform.right * x + transform.up * z;
        }
        // else
        // {
        //     move = transform.right * x + transform.forward * z;
        // }

        // sticks the player onto the train
        if (onTrain)
        {
            StartCoroutine(SetFaceActive());
            Vector3 trainMove = Vector3.MoveTowards(gameObject.transform.position, train.transform.position, Time.deltaTime) - train.transform.position;
            trainMove.x = -trainMove.x;
            trainMove.y = 0f;
            trainMove.z = -trainMove.z;
            move += trainMove;
        }


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
            controller.height = 1.2f;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftControl) && PoseParser.GETGestureAsString().CompareTo("C")!=0) && crouching)
        {
            crouching = false;
            controller.height = 1.8f;
        }

        //velocity.y += gravity * Time.deltaTime;
        // Only applies gravity when not climbing, this allows players to stay on ladder
        if (!climbing)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
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
            // Debug.Log("PLAYER ENTERED LADDER");
            train = other.gameObject;
            ladderCentreLine = ((BoxCollider) other).center;
            // Debug.Log("LADDER COORDS" + (train.transform.position + ladderCentreLine).ToString());
            climbing = true;
            //GetComponent<PlayerAnimation>().setClimbing(climbing);
            LeftHandUpUI.SetActive(true);
            RightHandUpUI.SetActive(true);
        }
        else if (other.gameObject.tag == "ladder")
        {
            climbingBuilding = true;
            GetComponent<PlayerAnimation>().setClimbing(climbingBuilding);
        }
        else if (other.gameObject.tag == "trainfloor")
        {
            //Debug.Log("stef is aiiiir");
            train = other.gameObject;
            climbing = false;
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
            photonView.RPC(nameof(ChangeOnTrainToTrue), RpcTarget.All);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (climbing && other.gameObject.tag == "locomotive")
        {
            // Debug.Log("player stopped climbing");
            climbing = false;
            //GetComponent<PlayerAnimation>().setClimbing(climbing);
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
        }
        else if (climbingBuilding && other.gameObject.tag == "ladder")
        {
            climbingBuilding = false;
            GetComponent<PlayerAnimation>().setClimbing(climbingBuilding);
        }
        else if (onTrain && other.gameObject.tag == "trainfloor")
        {
            train = null;
            photonView.RPC(nameof(ChangeOnTrainToFalse), RpcTarget.All);
            Debug.Log("stef is NOT aiiiir");
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
            Debug.Log("SET SPEED" + val);
            this.speed = val;
        }
    }

    public void setGrounded(bool val) {
        this.isGrounded = val;
    }
}
