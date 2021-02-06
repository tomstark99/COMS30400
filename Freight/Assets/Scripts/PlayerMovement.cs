using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    private float gravity = -9.8f;
    private float speed = 12f;
    private float jumpHeight = 2.5f;

    private Vector3 velocity;
    private float groundDistance = 0.4f;
    private bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // Checks if jump button is pressed and allows user to jump if they are on the ground
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
