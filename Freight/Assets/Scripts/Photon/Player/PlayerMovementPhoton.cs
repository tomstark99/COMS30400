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

    private float gravity = -17f;
    private float speed = 8f;
    private float jumpHeight = 3.5f;

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private Vector3 velocity;
    private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool climbing;
    private bool onTrain;
    private Transform prev;
    private PhotonView PV;

    public bool OnTrain
    {
        get { return onTrain; }
    }

   
    void Start()
    {
        // activates player's camera if its theirs and disables all others
        if(photonView.IsMine)
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

        //// if escape is pressed, quit game
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();
        
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

        Vector3 move;

        if (climbing && z > 0f)
        {
            move = transform.right * x + transform.up * z;
        }
        else
        {
            move = transform.right * x + transform.forward * z;
        }

        // sticks the player onto the train
        if (onTrain)
        {
            Vector3 trainMove = Vector3.MoveTowards(gameObject.transform.position, GameObject.FindGameObjectWithTag("locomotive").transform.position, Time.deltaTime) - GameObject.FindGameObjectWithTag("locomotive").transform.position;
            trainMove.x = -trainMove.x;
            trainMove.y = 0f;
            trainMove.z = -trainMove.z;
            move += trainMove;
        }

        controller.Move(move * speed * Time.deltaTime);

        // Checks if jump button is pressed and allows user to jump if they are on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            onTrain = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

   
    // trigger collider for the ladder and the train floor
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "locomotive")
        {
            Debug.Log("PLAYER ENTERED LADDER");
            climbing = true;
        }
        else if (other.gameObject.tag == "trainfloor")
        {
            Debug.Log("stef is aiiiir");
            onTrain = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (climbing && other.gameObject.tag == "locomotive")
        {
            Debug.Log("player stopped climbing");
            climbing = false;
        }
        else if (onTrain && other.gameObject.tag == "trainfloor")
        {
            Debug.Log("stef is NOT aiiiir");
            onTrain = false;
        }
    }
}
