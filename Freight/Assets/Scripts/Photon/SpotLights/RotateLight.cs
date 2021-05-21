using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
//Rotate Light is placed on the normal spotlights. It rotates the lights and sends an event to GuardAIPhoton if the player is inside the light. 
public class RotateLight : MonoBehaviour
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;

    [SerializeField]
    private float rotationMultiplier;
    public float rotationUpperLimit;
    public float rotationLowerLimit;

    public event Action PlayerInLight;

    [SerializeField]
    private float sightRange;
    [SerializeField]
    private float lightAngle;
    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private GameObject actualLight;
    [SerializeField]
    private GameObject pointlight;

    private GameObject[] players;

    public bool lightsTurnedOff;
    private bool lightsRotating;

    void Start()
    {
        positiveRotation = true;
        players = GameObject.FindGameObjectsWithTag("Player");
        lightsTurnedOff = false;
        lightsRotating = (bool)PhotonNetwork.CurrentRoom.CustomProperties["SpotlightsRotating"];

        

    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        //players need to be found every frame in case one of the player disconnects
        players = GameObject.FindGameObjectsWithTag("Player");

        //if lights hasnt been turned off using the laptop, rotate the lights
        if (!lightsTurnedOff && lightsRotating)
        {
            Rotate();
            PlayerSpotted();
        }
        else if (!lightsTurnedOff)
        {
            PlayerSpotted();
        }
    }

    //rotate the spotlights in between 2 angles
    void Rotate()
    {
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y > rotationUpperLimit)
            {

                positiveRotation = false;
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y < rotationLowerLimit)
            {
                positiveRotation = true;
            }
        }
    }

    //This function loops throught the player objects and checks if any of them are inside the lights
    void PlayerSpotted()
    {
        foreach (var player in players)
        {
            
            if (Vector3.Distance(transform.Find("pCylinder3/Point Light").position, player.transform.position) < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.Find("pCylinder3/Point Light").position).normalized;
            
                float guardPlayerAngle = Vector3.Angle(-transform.Find("pCylinder3/Point Light").right, dirToPlayer);
              
                if (guardPlayerAngle < lightAngle / 2f)
                {

                    // checks if spotlight line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the head 
                    if (!Physics.Linecast(transform.Find("pCylinder3/Point Light").transform.position, player.transform.Find("master/Reference/Hips/LeftUpLeg/LeftLeg/LeftFoot").transform.position, obstacleMask) || !Physics.Linecast(transform.Find("pCylinder3/Point Light").transform.position, player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask))
                    {
                       // Debug.Log("YE MANS GETTING DETECTED STIIIIIIIIIIIIIIIIIIIIIIIIIL");

                       //play detection sound and call event from guard ai
                        transform.parent.GetComponent<SpotlightSounds>()?.PlayDetectedSound();
                        PlayerInLight();
                    }
                }

            }

            // checks if player is in guard's view range 
        }
    }

    //draw red line if gizmos are drew
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.Find("pCylinder3/Point Light").position, -transform.Find("pCylinder3/Point Light").right * sightRange);
    }

    public void ToggleLights()
    {
        lightsTurnedOff = !lightsTurnedOff;
        actualLight.SetActive(!actualLight.activeSelf);
        pointlight.SetActive(!pointlight.activeSelf);
    }


}
