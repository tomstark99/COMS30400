﻿using System;
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
    private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool climbing;
    private bool crouching;
    private bool onTrain;
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
        if(photonView.IsMine)
        {
            // transform.Find("Camera").gameObject.SetActive(true);
            transform.Find("Camera/Camera").gameObject.SetActive(true);
            // transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head/Camera").gameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, -90.0f);
        }
        PV = GetComponent<PhotonView>();
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            Debug.Log(" DISABLE CONTROLER ");
            GetComponent<PlayerMovementPhoton>().enabled = false;
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

        // instantiates move with null so that it can be set depending on climb
        Vector3 move;

        // if forwards velocity is greater than 0 and inside the climbing collider, add to the vertical height instead of the forward height
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

        /*if (onMenu)
            return;*/

        controller.Move(move * speed * Time.deltaTime);

        // Checks if jump button is pressed and allows user to jump if they are on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            onTrain = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !crouching)
        {
            crouching = true;
            controller.height = 1.2f;
        } 
        else if (Input.GetKeyDown(KeyCode.LeftControl) && crouching)
        {
            crouching = false;
            controller.height = 1.8f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

   
    // trigger collider for the ladder and the train floor
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "locomotive" || other.gameObject.tag == "ladder")
        {
            Debug.Log("PLAYER ENTERED LADDER");
            climbing = true;
            GetComponent<PlayerAnimation>().setClimbing(climbing);
        }
        else if (other.gameObject.tag == "trainfloor")
        {
            onTrain = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (climbing && other.gameObject.tag == "locomotive" || other.gameObject.tag == "ladder")
        {
            Debug.Log("player stopped climbing");
            climbing = false;
            GetComponent<PlayerAnimation>().setClimbing(climbing);
        }
        else if (onTrain && other.gameObject.tag == "trainfloor")
        {
            Debug.Log("stef is NOT aiiiir");
            onTrain = false;
          
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
