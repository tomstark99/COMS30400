using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHitGroundAlert : MonoBehaviour
{
    public Transform groundCheck;
    private GameObject guards;
    private float groundDistance = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded ;
    private bool lastFrameValueOfIsGrounded;
    public bool rockHitGround;

    public event Action RockHitGround;

    // Start is called before the first frame update
    void Start()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        lastFrameValueOfIsGrounded = isGrounded;
        guards = GameObject.Find("Guards");
        Debug.Log(guards);
        foreach (Transform child in guards.transform)
        {

            
            float dist = Vector3.Distance(gameObject.transform.position, child.transform.position);
            //Debug.Log(dist);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded == true && lastFrameValueOfIsGrounded == false)
        {
            Debug.Log("Rock hit the ground alie");
            rockHitGround = true;
            //RockHitGround();
        }
        else
        {
            rockHitGround = false;
        }

        lastFrameValueOfIsGrounded = isGrounded;
    }
   
}
