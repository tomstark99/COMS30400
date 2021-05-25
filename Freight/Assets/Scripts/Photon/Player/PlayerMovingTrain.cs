using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script attached to the player. It allow a character controller to move on a moving rigid body (in out case, the train) 
public class PlayerMovingTrain : MonoBehaviour
{
    public Transform activePlatform;

    CharacterController controller;
    Vector3 moveDirection;
    Vector3 activeGlobalPlatformPoint;
    Vector3 activeLocalPlatformPoint;
    Quaternion activeGlobalPlatformRotation;
    Quaternion activeLocalPlatformRotation;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activePlatform != null)
        {
            Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
            moveDirection = newGlobalPlatformPoint - activeGlobalPlatformPoint;
            if (moveDirection.magnitude > 0.01f)
            {
                controller.Move(moveDirection);
            }
            if (activePlatform)
            {
                // Support moving platform rotation
                Quaternion newGlobalPlatformRotation = activePlatform.rotation * activeLocalPlatformRotation;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(activeGlobalPlatformRotation);
                // Prevent rotation of the local up vector
                rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                transform.rotation = rotationDiff * transform.rotation;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                UpdateMovingPlatform();
            }
        }
        else
        {
            if (moveDirection.magnitude > 0.01f)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime);
                controller.Move(moveDirection);
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Make sure we are really standing on a straight platform 
        // Not on the underside of one and not falling down from it either!
        if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41 && hit.gameObject.tag == "locomotive")
        {
            if (activePlatform != hit.collider.transform)
            {
                activePlatform = hit.collider.transform;
                UpdateMovingPlatform();
            }
        }
        else
        {
            activePlatform = null;
        }
    }

    void UpdateMovingPlatform()
    {
        activeGlobalPlatformPoint = transform.position;
        activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
        // Support moving platform rotation
        activeGlobalPlatformRotation = transform.rotation;
        activeLocalPlatformRotation = Quaternion.Inverse(activePlatform.rotation) * transform.rotation;
    }
}
