using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAnimation : MonoBehaviourPun
{
    // animation variables
    private Animator animator;
    private int isWalkingHash;
    private int isRunningHash;
    private int isRunningBackHash;
    private int isJumpingHash;
    private int isLeftHash;
    private int isRightHash;
    private int isCrouchedHash;
    private int isClimbingHash;

    private bool isGrounded;
    private bool climbing;
    private float runningSpeed;
    private float walkingSpeed;

    public GameObject camera;
    public GameObject head;

    private PlayerMovementPhoton player;

    // Start is called before the first frame update
    void Start()
    {
        // get grounded state from movement controller
        player = GetComponent<PlayerMovementPhoton>();
        isGrounded = player.getGrounded();
        runningSpeed = player.getSpeedRunning();
        walkingSpeed = player.getSpeedWalking();

        // Setup animation variables
        animator = GetComponent<Animator>();

        // using StringToHash increases performance by nearly 50%
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isRunningBackHash = Animator.StringToHash("isRunningBack");
        isJumpingHash = Animator.StringToHash("isJumping");
        isLeftHash = Animator.StringToHash("walkLeft");
        isRightHash = Animator.StringToHash("walkRight");
        isCrouchedHash = Animator.StringToHash("crouched");
        isClimbingHash = Animator.StringToHash("isClimbing");
    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        // animate function
        if (photonView.IsMine)
        {
            Animate();
            camera.transform.position = head.transform.position;
        }
        
    }

    private void Animate() {

        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isRunningBack = animator.GetBool(isRunningBackHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isLeft = animator.GetBool(isLeftHash);
        bool isRight = animator.GetBool(isRightHash);
        bool isCrouched = animator.GetBool(isCrouchedHash);
        bool isClimbing = animator.GetBool(isClimbingHash);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Debug.Log(climbing + " XDDD " + Input.GetKeyDown(KeyCode.W));


        if(climbing && Input.GetKey(KeyCode.W)) {
            animator.SetBool(isClimbingHash, true);
        } else {
            animator.SetBool(isClimbingHash, false);
        }

        if (!isWalking && z > 0.02f) {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool(isRunningHash, true);
                player.setSpeed(runningSpeed);
            }

            else
            {
                animator.SetBool(isWalkingHash, true);
                player.setSpeed(walkingSpeed);
            }
                
        } 
        if(!isRunningBack && z < -0.02f) {
            animator.SetBool(isRunningBackHash, true);
        }
        if(z <= 0.02f && z >= -0.02f) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isRunningBackHash, false);
        }
        if (!isRight && x > 0.02f) {
            animator.SetBool(isRightHash, true);
        }
        if (x <= 0.02f && x >= -0.02f) {
            animator.SetBool(isLeftHash, false);
            animator.SetBool(isRightHash, false);
        }
        if (!isLeft && x < -0.02f) {
            animator.SetBool(isLeftHash, true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && (isWalking || z > 0.02f)) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, true);
            player.setSpeed(runningSpeed);

        } else if (Input.GetKeyUp(KeyCode.LeftShift) && isRunning) {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isRunningHash, false);
            player.setSpeed(walkingSpeed);
        }

        if (Input.GetButtonDown("Jump") || !isGrounded) {
            animator.SetBool(isJumpingHash, true);
            // animator.SetBool(isRunningHash, false);
        } else if (isGrounded) {
            animator.SetBool(isJumpingHash, false);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouched) {
            animator.SetBool(isCrouchedHash, true);
        } else if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouched) {
            animator.SetBool(isCrouchedHash, false);
        }

    }

    public bool getGrounded() {
        return this.isGrounded;
    }
    public bool getClimbing() {
        return this.climbing;
    }

    public void setGrounded(bool val) {
        this.isGrounded = val;
    }
    public void setClimbing(bool val) {
        this.climbing = val;
    }
}
