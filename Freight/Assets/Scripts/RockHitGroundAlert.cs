using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RockHitGroundAlert : NetworkBehaviour
{
    public Transform groundCheck;
    private GameObject guards;
    private float groundDistance = 1f;
    public LayerMask groundMask;
    private bool isGrounded ;
    private bool lastFrameValueOfIsGrounded;
    [SyncVar] public bool rockHitGround;

    [ServerCallback]
    // Start is called before the first frame update
    void Start()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        lastFrameValueOfIsGrounded = isGrounded;
        guards = GameObject.Find("Guards1");
        Debug.Log(guards);
        foreach (Transform child in guards.transform)
        {

            
            float dist = Vector3.Distance(gameObject.transform.position, child.transform.position);
            //Debug.Log(dist);
        }
    }

    [ServerCallback]
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded == true && lastFrameValueOfIsGrounded == false)
        {
            Debug.Log("Rock hit the ground alie");
            rockHitGround = true;
        }
        else
        {
            rockHitGround = false;
        }

        lastFrameValueOfIsGrounded = isGrounded;
    }
   
}
