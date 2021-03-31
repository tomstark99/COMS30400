using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class rotateLight : MonoBehaviour
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;
    private float rotationMultiplier = 10f;
    public float rotationUpperLimit;
    public float rotationLowerLimit;

    public event Action PlayerInLight;

    [SerializeField]
    private float sightRange;
    [SerializeField]
    private float lightAngle;
    [SerializeField]
    private LayerMask obstacleMask;

    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {
        positiveRotation = true;
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y < rotationUpperLimit && spotlight.transform.eulerAngles.y > rotationLowerLimit)
            {
                if (positiveRotation == true)
                {
                    positiveRotation = false;
                }
                else
                {
                    positiveRotation = true;
                }
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y < rotationUpperLimit && spotlight.transform.eulerAngles.y > rotationLowerLimit)
            {
                if (positiveRotation == true)
                {
                    positiveRotation = false;
                }
                else
                {
                    positiveRotation = true;
                }
            }
        }

        PlayerSpotted();
    }

    void PlayerSpotted()
    {
        foreach (var player in players)
        {
            Debug.Log(Vector3.Distance(transform.Find("pCylinder3/Point Light").position, player.transform.position));
            if (Vector3.Distance(transform.Find("pCylinder3/Point Light").position, player.transform.position) < sightRange)
            {
                // vector from guard to player
                Vector3 dirToPlayer = (player.transform.position - transform.Find("pCylinder3/Point Light").position).normalized;
                Debug.Log("dirToPlayer" + dirToPlayer);
                // might have to change the -transform.Find("pCylinder3/Point Light").right
                float guardPlayerAngle = Vector3.Angle(-transform.Find("pCylinder3/Point Light").right, dirToPlayer);
                Debug.Log("guardPlayerAngle" + dirToPlayer);
                Debug.Log("lightAngle / 2f" + lightAngle / 2f);
                if (guardPlayerAngle < lightAngle / 2f)
                {
                    Debug.Log("YE MANS IN THE ANGLE STILL");
                    // checks if guard line of sight is blocked by an obstacle
                    // because player.transform.position checks a line to the player's feet, i also added a check on the second child (cube) so it checks if it can see his feet and the bottom of the cube
                    if (!Physics.Linecast(transform.Find("pCylinder3/Point Light").transform.position, player.transform.Find("master/Reference/Hips/LeftUpLeg/LeftLeg/LeftFoot").transform.position, obstacleMask) || !Physics.Linecast(transform.Find("pCylinder3/Point Light").transform.position, player.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head").transform.position, obstacleMask))
                    {
                        PlayerInLight();
                    }
                }

            }

            // checks if player is in guard's view range 
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.Find("pCylinder3/Point Light").position, -transform.Find("pCylinder3/Point Light").right * sightRange);
    }


}