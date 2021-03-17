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
    private int isRunningBackHash;
    private int isJumpingHash;
    private int isLeftHash;
    private int isRightHash;
    private int isCrouchedHash;

    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        // get grounded state from movement controller
        isGrounded = GetComponent<PlayerMovementPhoton>().getGrounded();

        // Setup animation variables
        animator = GetComponent<Animator>();

        // using StringToHash increases performance by nearly 50%
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningBackHash = Animator.StringToHash("isRunningBack");
        isJumpingHash = Animator.StringToHash("isJumping");
        isLeftHash = Animator.StringToHash("walkLeft");
        isRightHash = Animator.StringToHash("walkRight");
        isCrouchedHash = Animator.StringToHash("crouched");
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
            Animate();
        
    }

    private void Animate() {

        bool isJumping = animator.GetBool(isJumpingHash);
        bool isRunningBack = animator.GetBool(isRunningBackHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isLeft = animator.GetBool(isLeftHash);
        bool isRight = animator.GetBool(isRightHash);
        bool isCrouched = animator.GetBool(isCrouchedHash);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if(!isWalking && z > 0.02f) {
            animator.SetBool(isWalkingHash, true);
        } 
        if(!isRunningBack && z < -0.02f) {
            animator.SetBool(isRunningBackHash, true);
        }
        if(z <= 0.02f && z >= -0.02f) {
            animator.SetBool(isWalkingHash, false);
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

        if (Input.GetButtonDown("Jump") || !isGrounded) {
            animator.SetBool(isJumpingHash, true);
            // animator.SetBool(isWalkingHash, false);
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
    public void setGrounded(bool val) {
        this.isGrounded = val;
    }
}
