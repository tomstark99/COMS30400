using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHitGroundAlert : MonoBehaviour
{
    public Transform groundCheck;
    private float groundDistance = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded ;
    private bool lastFrameValueOfIsGrounded;
    public bool rockHitGround;

    public event Action RockHitGround;

    // Start is called before the first frame update
    void Start()
    {
        // checks if the rock hit the ground by checking a sphere around it
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // this makes sure to only call rock hitting the ground if the previous frame had the rock not hitting the ground
        lastFrameValueOfIsGrounded = isGrounded;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // if the rock has hit the ground and it wasn't hitting the ground in the last frame, call event to say that it hit the ground
        if (isGrounded == true && lastFrameValueOfIsGrounded == false)
        {
            //Debug.Log("Rock hit the ground alie");
            rockHitGround = true;
            // calls event
            if (RockHitGround != null)
                RockHitGround();
        }
        else
        {
            rockHitGround = false;
        }

        // update last frame value to current frame
        lastFrameValueOfIsGrounded = isGrounded;
    }
   
}
