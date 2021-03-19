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
    private float speed = 8f;
    private float jumpHeight = 3.5f;

    private Vector3 velocity;
    private Vector3 ladderCentreLine;
    private GameObject train;
    private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool climbing;
    private bool onTrain;
    private bool check = false;
    private Transform prev;
    private PhotonView PV;


    void Start()
    {
        // activates player's camera if its theirs and disables all others
        if (photonView.IsMine)
        {
            transform.Find("Cube").GetChild(0).gameObject.SetActive(true);
        }

        PV = GetComponent<PhotonView>();
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            Debug.Log(" DISABLE CONTROLER ");
            Destroy(GetComponent<PlayerMovementPhoton>());
        }
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        // movement function
        if (photonView.IsMine)
            Movement();

        // if escape is pressed, quit game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    IEnumerator SetFaceActive(){
        if(check == false){
            faceUI.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        if(Input.GetKeyDown(KeyCode.C) || PoseParser.GETGestureAsString().CompareTo("C") == 0){
            faceUI.SetActive(false);
            check = true;
        }
    }

    void Movement()
    {
        // Checks if the groundCheck object is within distance to the ground layer
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Prevents downward velocity from decreasing infinitely if player is on the ground
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float l = Input.GetAxis("Ladder");

        Vector3 move;
        
#if UNITY_WEBGL && !UNITY_EDITOR

        if (climbing && PoseParser.GETGestureAsString().CompareTo("L") == 0)
        {
            move = transform.up * 0.2f;
        } else if (climbing && l > 0f)
        {
            move = transform.up * l;
        } 
        else
        {
            move = transform.right * x + transform.forward * z;
        }
        
#else
        
        if (climbing && l > 0f)
        {
            move = transform.up * l;
            // move = transform.right * x + transform.up * z;
        }
        else
        {
            move = transform.right * x + transform.forward * z;
        }
        
#endif

        // sticks the player onto the train
        if (onTrain)
        {
            StartCoroutine(SetFaceActive());
            Vector3 trainMove = Vector3.MoveTowards(gameObject.transform.position, GameObject.FindGameObjectWithTag("locomotive").transform.position, Time.deltaTime) - GameObject.FindGameObjectWithTag("locomotive").transform.position;
            trainMove.x = -trainMove.x;
            trainMove.y = 0f;
            trainMove.z = -trainMove.z;
            move += trainMove;
        }

        if (climbing)
        {
            faceUI.SetActive(false);
            Vector3 ladderPos = train.transform.position + (train.transform.rotation * ladderCentreLine);
            ladderPos.y = transform.position.y;
            move += ladderPos - transform.position;
        }

        controller.Move(move * speed * Time.deltaTime);

        // Checks if jump button is pressed and allows user to jump if they are on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            onTrain = false;
            faceUI.SetActive(false);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Only applies gravity when not climbing, this allows players to stay on ladder
        if (!climbing)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

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
            //faceUI.SetActive(false);
            LeftHandUpUI.SetActive(true);
            RightHandUpUI.SetActive(true);
        }
        else if (other.gameObject.tag == "trainfloor")
        {
            Debug.Log("stef is aiiiir");
            climbing = false;
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
            onTrain = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (climbing && other.gameObject.tag == "locomotive")
        {
            // Debug.Log("player stopped climbing");
            climbing = false;
            LeftHandUpUI.SetActive(false);
            RightHandUpUI.SetActive(false);
        }
        else if (onTrain && other.gameObject.tag == "trainfloor")
        {
            Debug.Log("stef is NOT aiiiir");
            onTrain = false;
        }
    }
}
