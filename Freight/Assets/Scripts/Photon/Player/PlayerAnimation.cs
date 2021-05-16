using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAnimation : MonoBehaviourPun
{
    // animation variables
    public Animator animator;
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
    private bool isCrouched;
    private float runningSpeed;
    private float walkingSpeed;
    private bool crouching;
    
    public GameObject camera;
    public GameObject head;

    // public event Action ChangeAnimationLayer;

    public PlayerMovementPhoton player;

    // Start is called before the first frame update
    void Start()
    {
        // get grounded state from movement controller
        // player = GetComponent<PlayerMovementPhoton>();
        isGrounded = player.getGrounded();
        runningSpeed = player.getSpeedRunning();
        walkingSpeed = player.getSpeedWalking();

        crouching = false;

        // Setup animation variables
        // animator = GetComponent<Animator>();

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

    public void SetAllFalse()
    {
        animator.SetBool(isClimbingHash, false);
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isRunningHash, false);
        animator.SetBool(isRunningBackHash, false);
        animator.SetBool(isLeftHash, false);
        animator.SetBool(isRightHash, false);
        animator.SetBool(isCrouchedHash, false);
    }

    private void Animate() {

        //Debug.Log("____");

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
        //Debug.Log("Initial isCrouched:"+crouching);
        // Debug.Log(climbing + " XDDD " + Input.GetKeyDown(KeyCode.W));

        if (climbing && (Input.GetButton("Ladder") || PoseParser.GETGestureAsString().CompareTo("L") == 0)) {
            animator.SetBool(isClimbingHash, true);
        } else {
            animator.SetBool(isClimbingHash, false);
        }

        if (!isWalking && (z > 0.02f || PoseParser.GETGestureAsString().CompareTo("F") == 0)) {
            if (Input.GetKey(KeyCode.LeftShift) || PoseParser.GETGestureAsString().CompareTo("F") == 0)
            {
                //Debug.Log("running");
                animator.SetBool(isRunningHash, true);
                player.setSpeed(runningSpeed);
            }

            else if (!crouching)
            {
                //Debug.Log("walking");
                animator.SetBool(isWalkingHash, true);
                player.setSpeed(walkingSpeed);
            }

        }
        if (!isRunningBack && z < -0.02f) {
            animator.SetBool(isRunningBackHash, true);
        }
        if (z <= 0.02f && z >= -0.02f && PoseParser.GETGestureAsString().CompareTo("F") != 0) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isRunningBackHash, false);
        }
        if ((!isRight && x > 0.02f) || (!isRight && PoseParser.GETGestureAsString().CompareTo("I") == 0)) {
            animator.SetBool(isRightHash, true);
        }
        if (x <= 0.02f && x >= -0.02f && PoseParser.GETGestureAsString().CompareTo("I") != 0 && PoseParser.GETGestureAsString().CompareTo("O") != 0) {
            animator.SetBool(isLeftHash, false);
            animator.SetBool(isRightHash, false);
        }
        if ((!isLeft && x < -0.02f) || (!isLeft && PoseParser.GETGestureAsString().CompareTo("O") == 0)) {
            animator.SetBool(isLeftHash, true);
        }

        // if ((Input.GetKeyDown(KeyCode.LeftShift) || PoseParser.GETGestureAsString().CompareTo("F") == 0) && isCrouched) {
        //     animator.SetBool(isCrouchedHash, false);
        // }

        if (Input.GetKeyDown(KeyCode.LeftShift) && (isWalking || z > 0.02f)) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isCrouchedHash, false);
            animator.SetBool(isRunningHash, true);
            player.setSpeed(runningSpeed);

        } else if (Input.GetKeyUp(KeyCode.LeftShift) && isRunning) {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isRunningHash, false);
            player.setSpeed(walkingSpeed);
        }
        // } else if(PoseParser.GETGestureAsString().CompareTo("N")==0 && isRunning){
        //     animator.SetBool(isWalkingHash, false);
        //     animator.SetBool(isRunningHash, false);
        //     player.setSpeed(walkingSpeed);
        // }

        // if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouched && !isRunning) {
        //     animator.SetBool(isCrouchedHash, true);
        // } else if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouched) {
        //     animator.SetBool(isCrouchedHash, false);
        // }

        if ((Input.GetButtonDown("Jump") || !isGrounded) && !climbing) {
            animator.SetBool(isJumpingHash, true);
            // animator.SetBool(isRunningHash, false);
        } else if (isGrounded) {
            animator.SetBool(isJumpingHash, false);
        }

        if ((Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("C") == 0) && !crouching && !isRunning) {
            crouching = true;
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isCrouchedHash, true);
            
        } else if ((Input.GetKeyDown(KeyCode.LeftControl) || PoseParser.GETGestureAsString().CompareTo("N") == 0) && crouching) {
            crouching = false;
           // Debug.Log("uncrouch");
            animator.SetBool(isCrouchedHash, false);
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }

        if(z>=0.02f && crouching){
            animator.SetBool(isCrouchedHash, true);
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isRunningHash, false);

        } else if((z <= 0.02f && z >= -0.02f) && crouching){
            animator.SetBool(isCrouchedHash, true);
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }

    }

    public bool getGrounded() {
        return this.isGrounded;
    }
    public bool getClimbing() {
        return this.climbing;
    }
    public bool getCrouched() {
        return this.isCrouched;
    }

    public void setGrounded(bool val) {
        this.isGrounded = val;
    }
    public void setClimbing(bool val) {
        this.climbing = val;
    }

    public void ChangeLayerPistol() {
        //Debug.Log("accc pistol tings");
        animator.SetLayerWeight(0, 0.0f);
        animator.SetLayerWeight(1, 1.0f);
    }

    public void ChangeLayerDefault() {
        //Debug.Log("acc default tings");
        animator.SetLayerWeight(0, 1.0f);
        animator.SetLayerWeight(1, 0.0f);
    }
}
